import { useDroppable } from '@dnd-kit/core';
import { SortableContext, verticalListSortingStrategy } from '@dnd-kit/sortable';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import TaskCard from './TaskCard';
import type { ProjectStatus, TaskItem } from '@/api/types';

interface KanbanColumnProps {
  status: ProjectStatus;
  tasks: TaskItem[];
  onAddTask: () => void;
}

export default function KanbanColumn({ status, tasks, onAddTask }: KanbanColumnProps) {
  const { setNodeRef, isOver } = useDroppable({ id: status.id });

  return (
    <div className="flex flex-col w-72 shrink-0 h-full">
      {/* Header */}
      <div className="flex items-center gap-2 mb-3 px-1">
        <span
          className="w-2.5 h-2.5 rounded-full shrink-0"
          style={{ backgroundColor: status.color }}
        />
        <span className="text-sm font-semibold text-gray-700 truncate">{status.name}</span>
        <Badge variant="secondary" className="ml-auto text-xs">
          {tasks.length}
        </Badge>
      </div>

      {/* Task list drop zone */}
      <div
        ref={setNodeRef}
        className={`flex-1 overflow-y-auto rounded-lg p-2 space-y-2 min-h-[120px] transition-colors ${
          isOver ? 'bg-blue-50 ring-2 ring-blue-200' : 'bg-gray-100'
        }`}
      >
        <SortableContext
          items={tasks.map((t) => t.id)}
          strategy={verticalListSortingStrategy}
        >
          {tasks.map((task) => (
            <TaskCard key={task.id} task={task} />
          ))}
        </SortableContext>
      </div>

      {/* Footer */}
      <Button
        variant="ghost"
        size="sm"
        className="mt-2 w-full justify-start text-gray-500 hover:text-gray-800 text-xs"
        onClick={onAddTask}
      >
        + Add task
      </Button>
    </div>
  );
}
