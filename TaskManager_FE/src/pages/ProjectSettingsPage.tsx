import { useState, useEffect } from 'react';
import { useParams, NavLink, Link } from 'react-router-dom';
import { X, Plus } from 'lucide-react';
import axios from 'axios';
import { useProject, useUpdateProject } from '@/api/projects';
import { useProjectCategories, useCreateCategory, useDeleteCategory } from '@/api/expenses';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';

export default function ProjectSettingsPage() {
  const { id } = useParams<{ id: string }>();
  const { data: project } = useProject(id!);
  const { data: categories = [] } = useProjectCategories(id!);

  const updateProject = useUpdateProject(id!);
  const createCategory = useCreateCategory(id!);
  const deleteCategory = useDeleteCategory(id!);

  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [budget, setBudget] = useState('');
  const [saved, setSaved] = useState(false);

  useEffect(() => {
    if (project) {
      setName(project.name);
      setDescription(project.description ?? '');
      setBudget(String(project.budget));
    }
  }, [project]);

  async function handleSaveDetails() {
    await updateProject.mutateAsync({ name, description: description || undefined, budget: Number(budget) });
    setSaved(true);
    setTimeout(() => setSaved(false), 2000);
  }

  const [newCatName, setNewCatName] = useState('');
  const [addingCat, setAddingCat] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  async function handleAddCategory() {
    if (!newCatName.trim()) return;
    await createCategory.mutateAsync(newCatName.trim());
    setNewCatName('');
    setAddingCat(false);
  }

  async function handleDeleteCategory(catId: string) {
    setDeleteError(null);
    try {
      await deleteCategory.mutateAsync(catId);
    } catch (err) {
      if (axios.isAxiosError(err) && err.response?.status === 409) {
        setDeleteError(err.response.data?.error ?? 'This group has expenses assigned and cannot be deleted.');
      }
    }
  }

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

      <div className="flex-1 overflow-auto p-6 max-w-lg">
        <section className="mb-8">
          <h3 className="text-sm font-semibold text-gray-900 mb-4">Project details</h3>

          <div className="flex flex-col gap-3">
            <div>
              <label className="text-xs font-medium text-gray-600 mb-1 block">Name</label>
              <Input
                value={name}
                onChange={(e) => setName(e.target.value)}
                className="h-8 text-sm"
              />
            </div>

            <div>
              <label className="text-xs font-medium text-gray-600 mb-1 block">Description</label>
              <Textarea
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Optional description…"
                className="text-sm resize-none"
                rows={3}
              />
            </div>

            <div>
              <label className="text-xs font-medium text-gray-600 mb-1 block">Budget (CZK)</label>
              <Input
                type="number"
                min={0}
                value={budget}
                onChange={(e) => setBudget(e.target.value)}
                className="h-8 text-sm"
              />
            </div>

            <div className="flex items-center gap-3 mt-1">
              <Button
                size="sm"
                onClick={handleSaveDetails}
                disabled={updateProject.isPending || !name.trim()}
              >
                Save
              </Button>
              {saved && <span className="text-xs text-green-600">Saved</span>}
            </div>
          </div>
        </section>

        <section>
          <h3 className="text-sm font-semibold text-gray-900 mb-1">Expense groups</h3>
          <p className="text-sm text-gray-500 mb-4">Groups let you categorise expenses on the Expenses tab.</p>

          <div className="flex flex-col gap-2 mb-3">
            {categories.length === 0 && (
              <p className="text-sm text-gray-400 italic">No groups yet.</p>
            )}
            {categories.map((cat) => (
              <div key={cat.id} className="flex items-center justify-between px-3 py-2 rounded-md border bg-white">
                <span className="text-sm text-gray-800">{cat.name}</span>
                <button
                  onClick={() => handleDeleteCategory(cat.id)}
                  className="text-gray-400 hover:text-red-500 transition-colors p-0.5 rounded"
                  aria-label={`Delete ${cat.name}`}
                >
                  <X size={14} />
                </button>
              </div>
            ))}
          </div>

          {deleteError && (
            <p className="text-sm text-red-600 mb-3">{deleteError}</p>
          )}

          {addingCat ? (
            <div className="flex items-center gap-2">
              <Input
                autoFocus
                value={newCatName}
                onChange={(e) => setNewCatName(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') handleAddCategory();
                  if (e.key === 'Escape') { setAddingCat(false); setNewCatName(''); }
                }}
                placeholder="Group name"
                className="h-8 text-sm"
              />
              <Button size="sm" onClick={handleAddCategory} disabled={!newCatName.trim() || createCategory.isPending}>
                Add
              </Button>
              <Button size="sm" variant="ghost" onClick={() => { setAddingCat(false); setNewCatName(''); }}>
                Cancel
              </Button>
            </div>
          ) : (
            <button
              onClick={() => setAddingCat(true)}
              className="inline-flex items-center gap-1.5 text-sm text-gray-500 hover:text-gray-800 transition-colors"
            >
              <Plus size={14} /> Add group
            </button>
          )}
        </section>
      </div>
    </div>
  );
}
