import { useState } from 'react';
import { useParams, NavLink, Link } from 'react-router-dom';
import { Trash2, Pencil } from 'lucide-react';
import { useProject } from '@/api/projects';
import {
  useProjectExpenses,
  useCreateExpense,
  useUpdateExpense,
  useDeleteExpense,
  useProjectCategories,
} from '@/api/expenses';
import type { Expense } from '@/api/types';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';

const UNCATEGORIZED = '__uncategorized__';

interface ExpenseFormState {
  amount: string;
  description: string;
  date: string;
  categoryId: string;
}

function emptyForm(): ExpenseFormState {
  return { amount: '', description: '', date: new Date().toISOString().slice(0, 10), categoryId: '' };
}

function formFromExpense(e: Expense): ExpenseFormState {
  return {
    amount: String(e.amount),
    description: e.description ?? '',
    date: new Date(e.date).toISOString().slice(0, 10),
    categoryId: e.categoryId ?? '',
  };
}

// ── Sub-components ────────────────────────────────────────────────────────────

function ExpenseRow({
  expense, currency, fmt, onEdit, onDelete,
}: {
  expense: Expense;
  currency: string;
  fmt: (n: number) => string;
  onEdit: (e: Expense) => void;
  onDelete: (id: string) => void;
}) {
  return (
    <div onDoubleClick={() => onEdit(expense)} className="flex items-center gap-3 px-4 py-2.5 bg-white hover:bg-gray-50 group border-b last:border-b-0 cursor-pointer">
      <span className="text-gray-400 text-sm w-24 shrink-0">
        {new Date(expense.date).toLocaleDateString('cs-CZ')}
      </span>
      <span className="flex-1 text-sm text-gray-900 min-w-0 truncate">
        {expense.description ?? <span className="text-gray-400 italic">No description</span>}
      </span>
      <span className="text-sm font-medium text-gray-900 shrink-0">
        {currency} {fmt(expense.amount)}
      </span>
      <div className="flex items-center gap-0.5 opacity-0 group-hover:opacity-100 transition-opacity shrink-0">
        <button onClick={() => onEdit(expense)} className="p-1 rounded text-gray-400 hover:text-blue-500 transition-colors" aria-label="Edit">
          <Pencil size={14} />
        </button>
        <button onClick={() => onDelete(expense.id)} className="p-1 rounded text-gray-400 hover:text-red-500 transition-colors" aria-label="Delete">
          <Trash2 size={14} />
        </button>
      </div>
    </div>
  );
}

function ExpenseGroup({
  label, total, expenses, currency, fmt, onEdit, onDelete,
}: {
  label: string;
  total: number;
  expenses: Expense[];
  currency: string;
  fmt: (n: number) => string;
  onEdit: (e: Expense) => void;
  onDelete: (id: string) => void;
}) {
  const sorted = [...expenses].sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());
  return (
    <div className="border rounded-lg overflow-hidden">
      <div className="flex items-center justify-between px-4 py-2 bg-gray-50 border-b">
        <span className="text-sm font-medium text-gray-700">{label}</span>
        <span className="text-sm text-gray-500">{currency} {fmt(total)}</span>
      </div>
      {sorted.map((expense) => (
        <ExpenseRow key={expense.id} expense={expense} currency={currency} fmt={fmt} onEdit={onEdit} onDelete={onDelete} />
      ))}
    </div>
  );
}

// ── Page ──────────────────────────────────────────────────────────────────────

export default function ExpensesPage() {
  const { id } = useParams<{ id: string }>();
  const { data: project } = useProject(id!);
  const { data: expenses = [], isLoading } = useProjectExpenses(id!);
  const { data: categories = [] } = useProjectCategories(id!);

  const createExpense = useCreateExpense(id!);
  const updateExpense = useUpdateExpense(id!);
  const deleteExpense = useDeleteExpense(id!);

  const [editingExpense, setEditingExpense] = useState<Expense | null>(null);
  const [createOpen, setCreateOpen] = useState(false);
  const [form, setForm] = useState<ExpenseFormState>(emptyForm);

  const currency = project?.currency ?? 'CZK';
  const fmt = (n: number) => n.toLocaleString('cs-CZ', { minimumFractionDigits: 2 });

  function setField<K extends keyof ExpenseFormState>(k: K, v: ExpenseFormState[K]) {
    setForm((f) => ({ ...f, [k]: v }));
  }

  function openCreate() { setForm(emptyForm()); setCreateOpen(true); }
  function openEdit(expense: Expense) { setForm(formFromExpense(expense)); setEditingExpense(expense); }
  function closeDialog() { setCreateOpen(false); setEditingExpense(null); }

  async function handleCreate() {
    const parsed = parseFloat(form.amount);
    if (!parsed || parsed <= 0) return;
    await createExpense.mutateAsync({
      projectId: id!,
      categoryId: form.categoryId || undefined,
      amount: parsed,
      description: form.description.trim() || undefined,
      date: new Date(form.date).toISOString(),
    });
    closeDialog();
  }

  async function handleUpdate() {
    if (!editingExpense) return;
    const parsed = parseFloat(form.amount);
    if (!parsed || parsed <= 0) return;
    await updateExpense.mutateAsync({
      expenseId: editingExpense.id,
      input: {
        categoryId: form.categoryId || undefined,
        amount: parsed,
        description: form.description.trim() || undefined,
        date: new Date(form.date).toISOString(),
      },
    });
    closeDialog();
  }

  const total = expenses.reduce((sum, e) => sum + e.amount, 0);
  const budget = project?.budget ?? 0;
  const remaining = budget - total;

  const grouped = new Map<string, Expense[]>();
  for (const e of expenses) {
    const key = e.categoryId ?? UNCATEGORIZED;
    if (!grouped.has(key)) grouped.set(key, []);
    grouped.get(key)!.push(e);
  }

  const orderedKeys = [
    ...categories.map((c) => c.id).filter((cid) => grouped.has(cid)),
    ...(grouped.has(UNCATEGORIZED) ? [UNCATEGORIZED] : []),
  ];

  const isEditing = editingExpense !== null;
  const dialogOpen = createOpen || isEditing;
  const isSaving = createExpense.isPending || updateExpense.isPending;

  return (
    <div className="flex flex-col h-full">
      <div className="px-6 pt-6 pb-0 shrink-0">
        <Link to="/projects" className="text-sm text-gray-500 hover:text-gray-800 transition-colors mb-3 inline-block">
          ← Projects
        </Link>
        <h2 className="text-2xl font-semibold text-gray-900">{project?.name ?? 'Loading…'}</h2>

        <nav className="flex gap-1 mt-4 border-b border-gray-200">
          {[
            { label: 'Board', to: `/projects/${id}` },
            { label: 'Expenses', to: `/projects/${id}/expenses` },
            { label: 'Settings', to: `/projects/${id}/settings` },
          ].map(({ label, to }) => (
            <NavLink key={to} to={to} end
              className={({ isActive }) =>
                `px-4 py-2 text-sm font-medium border-b-2 -mb-px transition-colors ${
                  isActive ? 'border-blue-600 text-blue-600' : 'border-transparent text-gray-500 hover:text-gray-800'
                }`
              }
            >
              {label}
            </NavLink>
          ))}
        </nav>
      </div>

      <div className="flex-1 overflow-auto p-6">
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-6">
          <Card>
            <CardHeader className="pb-1"><CardTitle className="text-sm font-medium text-gray-500">Total spent</CardTitle></CardHeader>
            <CardContent><p className="text-2xl font-semibold text-gray-900">{currency} {fmt(total)}</p></CardContent>
          </Card>
          <Card>
            <CardHeader className="pb-1"><CardTitle className="text-sm font-medium text-gray-500">Budget</CardTitle></CardHeader>
            <CardContent><p className="text-2xl font-semibold text-gray-900">{currency} {fmt(budget)}</p></CardContent>
          </Card>
          <Card>
            <CardHeader className="pb-1"><CardTitle className="text-sm font-medium text-gray-500">Remaining</CardTitle></CardHeader>
            <CardContent>
              <p className={`text-2xl font-semibold ${remaining < 0 ? 'text-red-600' : 'text-green-600'}`}>
                {currency} {fmt(remaining)}
              </p>
            </CardContent>
          </Card>
        </div>

        <div className="flex items-center justify-between mb-4">
          <h3 className="text-base font-medium text-gray-900">Transactions</h3>
          <Button size="sm" onClick={openCreate}>Add expense</Button>
        </div>

        {isLoading && <p className="text-sm text-gray-500">Loading…</p>}
        {!isLoading && expenses.length === 0 && <p className="text-sm text-gray-500">No expenses yet.</p>}

        <div className="space-y-4">
          {orderedKeys.map((key) => (
            <ExpenseGroup
              key={key}
              label={key === UNCATEGORIZED ? 'Uncategorized' : (categories.find((c) => c.id === key)?.name ?? key)}
              total={(grouped.get(key) ?? []).reduce((s, e) => s + e.amount, 0)}
              expenses={grouped.get(key) ?? []}
              currency={currency}
              fmt={fmt}
              onEdit={openEdit}
              onDelete={(expenseId) => deleteExpense.mutate(expenseId)}
            />
          ))}
        </div>
      </div>

      <Dialog open={dialogOpen} onOpenChange={(open) => !open && closeDialog()}>
        <DialogContent className="sm:max-w-sm">
          <DialogHeader>
            <DialogTitle>{isEditing ? 'Edit expense' : 'Add expense'}</DialogTitle>
          </DialogHeader>
          <div className="grid gap-4 py-2">
            <div className="grid gap-1.5">
              <Label htmlFor="exp-amount">Amount ({currency}) *</Label>
              <Input id="exp-amount" type="number" min="0.01" step="0.01" value={form.amount} onChange={(e) => setField('amount', e.target.value)} placeholder="0.00" />
            </div>
            <div className="grid gap-1.5">
              <Label htmlFor="exp-desc">Description</Label>
              <Input id="exp-desc" value={form.description} onChange={(e) => setField('description', e.target.value)} placeholder="What was this for?" />
            </div>
            <div className="grid gap-1.5">
              <Label htmlFor="exp-date">Date *</Label>
              <Input id="exp-date" type="date" value={form.date} onChange={(e) => setField('date', e.target.value)} />
            </div>
            <div className="grid gap-1.5">
              <Label htmlFor="exp-cat">Group</Label>
              <select
                id="exp-cat"
                value={form.categoryId}
                onChange={(e) => setField('categoryId', e.target.value)}
                className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
              >
                <option value="">None</option>
                {categories.map((cat) => <option key={cat.id} value={cat.id}>{cat.name}</option>)}
              </select>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={closeDialog}>Cancel</Button>
            <Button onClick={isEditing ? handleUpdate : handleCreate} disabled={!form.amount || parseFloat(form.amount) <= 0 || isSaving}>
              {isSaving ? 'Saving…' : isEditing ? 'Save' : 'Add'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
