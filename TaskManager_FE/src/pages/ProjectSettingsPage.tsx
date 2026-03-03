import { useState, useEffect } from 'react';
import { useParams, NavLink, Link } from 'react-router-dom';
import { X, Plus, Pencil, Check } from 'lucide-react';
import axios from 'axios';
import { useProject, useUpdateProject, useCreateStatus, useUpdateStatus, useDeleteStatus } from '@/api/projects';
import { useProjectCategories, useCreateCategory, useDeleteCategory } from '@/api/expenses';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import type { ProjectStatus } from '@/api/types';

export default function ProjectSettingsPage() {
  const { id } = useParams<{ id: string }>();
  const { data: project } = useProject(id!);
  const { data: categories = [] } = useProjectCategories(id!);

  const updateProject = useUpdateProject(id!);
  const createCategory = useCreateCategory(id!);
  const deleteCategory = useDeleteCategory(id!);
  const createStatus = useCreateStatus(id!);
  const updateStatus = useUpdateStatus(id!);
  const deleteStatus = useDeleteStatus(id!);

  // Project details state
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

  // Expense groups state
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

  // Statuses state
  const [editingStatus, setEditingStatus] = useState<ProjectStatus | null>(null);
  const [editName, setEditName] = useState('');
  const [editColor, setEditColor] = useState('');
  const [addingStatus, setAddingStatus] = useState(false);
  const [newStatusName, setNewStatusName] = useState('');
  const [newStatusColor, setNewStatusColor] = useState('#6B7280');
  const [statusDeleteError, setStatusDeleteError] = useState<string | null>(null);

  function startEditStatus(status: ProjectStatus) {
    setEditingStatus(status);
    setEditName(status.name);
    setEditColor(status.color);
  }

  async function handleSaveStatus() {
    if (!editingStatus || !editName.trim()) return;
    await updateStatus.mutateAsync({ statusId: editingStatus.id, name: editName.trim(), color: editColor });
    setEditingStatus(null);
  }

  async function handleAddStatus() {
    if (!newStatusName.trim()) return;
    await createStatus.mutateAsync({ name: newStatusName.trim(), color: newStatusColor });
    setNewStatusName('');
    setNewStatusColor('#6B7280');
    setAddingStatus(false);
  }

  async function handleDeleteStatus(statusId: string) {
    setStatusDeleteError(null);
    try {
      await deleteStatus.mutateAsync(statusId);
    } catch (err) {
      if (axios.isAxiosError(err) && err.response?.status === 409) {
        setStatusDeleteError(err.response.data?.error ?? 'This status has tasks assigned and cannot be deleted.');
      }
    }
  }

  const statuses = [...(project?.statuses ?? [])].sort((a, b) => a.order - b.order);

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

        <section className="mb-8">
          <h3 className="text-sm font-semibold text-gray-900 mb-1">Board statuses</h3>
          <p className="text-sm text-gray-500 mb-4">Statuses define the columns on the board.</p>

          <div className="flex flex-col gap-2 mb-3">
            {statuses.length === 0 && (
              <p className="text-sm text-gray-400 italic">No statuses yet.</p>
            )}
            {statuses.map((status) => (
              editingStatus?.id === status.id ? (
                <div key={status.id} className="flex items-center gap-2 px-3 py-2 rounded-md border bg-white">
                  <input
                    type="color"
                    value={editColor}
                    onChange={(e) => setEditColor(e.target.value)}
                    className="h-6 w-6 rounded cursor-pointer border-0 p-0"
                  />
                  <Input
                    autoFocus
                    value={editName}
                    onChange={(e) => setEditName(e.target.value)}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter') handleSaveStatus();
                      if (e.key === 'Escape') setEditingStatus(null);
                    }}
                    className="h-7 text-sm flex-1"
                  />
                  <button
                    onClick={handleSaveStatus}
                    disabled={!editName.trim() || updateStatus.isPending}
                    className="text-green-600 hover:text-green-700 p-0.5 rounded transition-colors"
                    aria-label="Save"
                  >
                    <Check size={14} />
                  </button>
                  <button
                    onClick={() => setEditingStatus(null)}
                    className="text-gray-400 hover:text-gray-600 p-0.5 rounded transition-colors"
                    aria-label="Cancel"
                  >
                    <X size={14} />
                  </button>
                </div>
              ) : (
                <div key={status.id} className="flex items-center justify-between px-3 py-2 rounded-md border bg-white group">
                  <div className="flex items-center gap-2">
                    <span className="h-3 w-3 rounded-full flex-shrink-0" style={{ backgroundColor: status.color }} />
                    <span className="text-sm text-gray-800">{status.name}</span>
                  </div>
                  <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                    <button
                      onClick={() => startEditStatus(status)}
                      className="text-gray-400 hover:text-blue-500 transition-colors p-0.5 rounded"
                      aria-label={`Edit ${status.name}`}
                    >
                      <Pencil size={13} />
                    </button>
                    <button
                      onClick={() => handleDeleteStatus(status.id)}
                      className="text-gray-400 hover:text-red-500 transition-colors p-0.5 rounded"
                      aria-label={`Delete ${status.name}`}
                    >
                      <X size={14} />
                    </button>
                  </div>
                </div>
              )
            ))}
          </div>

          {statusDeleteError && (
            <p className="text-sm text-red-600 mb-3">{statusDeleteError}</p>
          )}

          {addingStatus ? (
            <div className="flex items-center gap-2">
              <input
                type="color"
                value={newStatusColor}
                onChange={(e) => setNewStatusColor(e.target.value)}
                className="h-7 w-7 rounded cursor-pointer border border-gray-200 p-0.5"
              />
              <Input
                autoFocus
                value={newStatusName}
                onChange={(e) => setNewStatusName(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') handleAddStatus();
                  if (e.key === 'Escape') { setAddingStatus(false); setNewStatusName(''); }
                }}
                placeholder="Status name"
                className="h-8 text-sm"
              />
              <Button size="sm" onClick={handleAddStatus} disabled={!newStatusName.trim() || createStatus.isPending}>
                Add
              </Button>
              <Button size="sm" variant="ghost" onClick={() => { setAddingStatus(false); setNewStatusName(''); }}>
                Cancel
              </Button>
            </div>
          ) : (
            <button
              onClick={() => setAddingStatus(true)}
              className="inline-flex items-center gap-1.5 text-sm text-gray-500 hover:text-gray-800 transition-colors"
            >
              <Plus size={14} /> Add status
            </button>
          )}
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
