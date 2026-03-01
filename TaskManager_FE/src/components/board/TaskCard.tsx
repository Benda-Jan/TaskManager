import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import type { TaskItem } from '@/api/types';

interface TaskCardProps {
  task: TaskItem;
}

export default function TaskCard({ task }: TaskCardProps) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id: task.id,
  });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.4 : 1,
  };

  return (
    <div ref={setNodeRef} style={style} {...attributes} {...listeners}>
      <Card className="cursor-grab active:cursor-grabbing shadow-sm hover:shadow-md transition-shadow py-0">
        <CardContent className="p-3 space-y-2">
          <p className="text-sm font-medium text-gray-900 leading-snug">{task.title}</p>
          <div className="flex items-center justify-between">
            <Badge
              variant={task.type === 'Deadline' ? 'destructive' : 'secondary'}
              className="text-xs"
            >
              {task.type}
            </Badge>
            {task.endDate && (
              <span className="text-xs text-gray-400">
                {new Date(task.endDate).toLocaleDateString()}
              </span>
            )}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
