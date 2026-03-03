import { useState } from 'react';
import { useParams, NavLink, Link } from 'react-router-dom';
import {
  DndContext,
  DragOverlay,
  PointerSensor,
  useSensor,
  useSensors,
  type DragEndEvent,
  type DragStartEvent,
} from '@dnd-kit/core';
import { useProject } from '@/api/projects';
import { useProjectTasks, useCreateTask, useMoveTask } from '@/api/tasks';
import KanbanColumn from '@/components/board/KanbanColumn';
import TaskCard from '@/components/board/TaskCard';
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import type { TaskItem, ProjectStatus } from '@/api/types';

function resolveTargetStatus(
  overId: string | number,
  tasks: TaskItem[],
  statuses: ProjectStatus[]
): string | null {
  const id = String(overId);
  if (statuses.some((s) => s.id === id)) return id;
  const task = tasks.find((t) => t.id === id);
  return task ? task.statusId : null;
}

export default function ProjectBoardPage() {
  const { id } = useParams<{ id: string }>();
  const { data: project, isLoading: projectLoading } = useProject(id!);
  const { data: tasks = [], isLoading: tasksLoading } = useProjectTasks(id!);
  const createTask = useCreateTask();
  const moveTask = useMoveTask();

  const [activeTask, setActiveTask] = useState<TaskItem | null>(null);
  const [dialogStatusId, setDialogStatusId] = useState<string | null>(null);
  const [taskTitle, setTaskTitle] = useState('');
  const [taskDescription, setTaskDescription] = useState('');
  const [taskType, setTaskType] = useState<'Standard' | 'Deadline'>('Standard');

  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 5 } })
  );

  const statuses = project?.statuses
    ? [...project.statuses].sort((a, b) => a.order - b.order)
    : [];

  function handleDragStart(event: DragStartEvent) {
    const task = tasks.find((t) => t.id === event.active.id);
    setActiveTask(task ?? null);
  }

  function handleDragEnd(event: DragEndEvent) {
    setActiveTask(null);
    const { active, over } = event;
    if (!over || active.id === over.id) return;

    const targetStatusId = resolveTargetStatus(over.id, tasks, statuses);
    const task = tasks.find((t) => t.id === active.id);
    if (task && targetStatusId && task.statusId !== targetStatusId) {
      moveTask.mutate({ taskId: task.id, statusId: targetStatusId, projectId: id! });
    }
  }

  function openAddTask(statusId: string) {
    setDialogStatusId(statusId);
    setTaskTitle('');
    setTaskDescription('');
    setTaskType('Standard');
  }

  function closeDialog() {
    setDialogStatusId(null);
  }

  async function handleCreateTask() {
    if (!taskTitle.trim() || !dialogStatusId) return;
    await createTask.mutateAsync({
      projectId: id!,
      statusId: dialogStatusId,
      title: taskTitle.trim(),
      description: taskDescription.trim() || undefined,
      type: taskType,
    });
    closeDialog();
  }

  const isLoading = projectLoading || tasksLoading;

  return (
    <div className="flex flex-col h-full">
      {/* Page header */}
      <div className="px-6 pt-6 pb-0 shrink-0">
        <Link
          to="/projects"
          className="text-sm text-gray-500 hover:text-gray-800 transition-colors mb-3 inline-block"
        >
          ← Projects
        </Link>
        <h2 className="text-2xl font-semibold text-gray-900">
          {project?.name ?? 'Loading…'}
        </h2>

        {/* Sub-navigation */}
        <nav className="flex gap-1 mt-4 border-b border-gray-200">
          {[
            { label: 'Board', to: `/projects/${id}` },
            { label: 'Expenses', to: `/projects/${id}/expenses` },
            { label: 'Settings', to: `/projects/${id}/settings` },
          ].map(({ label, to }) => (
            <NavLink
              key={to}
              to={to}
              end
              className={({ isActive }) =>
                `px-4 py-2 text-sm font-medium border-b-2 -mb-px transition-colors ${
                  isActive
                    ? 'border-blue-600 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-800'
                }`
              }
            >
              {label}
            </NavLink>
          ))}
        </nav>
      </div>

      {/* Board area */}
      <div className="flex-1 min-h-0 overflow-x-auto overflow-y-hidden px-6 py-6">
        {isLoading && (
          <p className="text-sm text-gray-500">Loading board…</p>
        )}

        {!isLoading && statuses.length === 0 && (
          <p className="text-sm text-gray-500">
            No statuses configured for this project yet.
          </p>
        )}

        {!isLoading && statuses.length > 0 && (
          <DndContext
            sensors={sensors}
            onDragStart={handleDragStart}
            onDragEnd={handleDragEnd}
          >
            <div className="flex gap-4 h-full">
              {statuses.map((status) => (
                <KanbanColumn
                  key={status.id}
                  status={status}
                  tasks={tasks.filter((t) => t.statusId === status.id)}
                  onAddTask={() => openAddTask(status.id)}
                />
              ))}
            </div>

            <DragOverlay>
              {activeTask && <TaskCard task={activeTask} />}
            </DragOverlay>
          </DndContext>
        )}
      </div>

      {/* Create task dialog */}
      <Dialog open={!!dialogStatusId} onOpenChange={(open) => !open && closeDialog()}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Add task</DialogTitle>
          </DialogHeader>
          <div className="grid gap-4 py-2">
            <div className="grid gap-1.5">
              <Label htmlFor="task-title">Title *</Label>
              <Input
                id="task-title"
                value={taskTitle}
                onChange={(e) => setTaskTitle(e.target.value)}
                placeholder="Task title"
                onKeyDown={(e) => e.key === 'Enter' && handleCreateTask()}
              />
            </div>
            <div className="grid gap-1.5">
              <Label htmlFor="task-desc">Description</Label>
              <Textarea
                id="task-desc"
                value={taskDescription}
                onChange={(e) => setTaskDescription(e.target.value)}
                placeholder="Optional description"
                rows={3}
              />
            </div>
            <div className="grid gap-1.5">
              <Label htmlFor="task-type">Type</Label>
              <select
                id="task-type"
                value={taskType}
                onChange={(e) => setTaskType(e.target.value as 'Standard' | 'Deadline')}
                className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
              >
                <option value="Standard">Standard</option>
                <option value="Deadline">Deadline</option>
              </select>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={closeDialog}>
              Cancel
            </Button>
            <Button
              onClick={handleCreateTask}
              disabled={!taskTitle.trim() || createTask.isPending}
            >
              {createTask.isPending ? 'Adding…' : 'Add task'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
