export interface User {
  id: string;
  email: string;
  name: string;
}

export interface Project {
  id: string;
  name: string;
  description?: string;
  budget: number;
  currency: string;
  memberCount: number;
  createdAt: string;
}

export interface ProjectStatus {
  id: string;
  projectId: string;
  name: string;
  color: string;
  order: number;
}

export interface ProjectDetail extends Project {
  statuses: ProjectStatus[];
}

export interface CreateTaskInput {
  projectId: string;
  statusId: string;
  title: string;
  description?: string;
  type: 'Standard' | 'Deadline';
}

export interface MoveTaskInput {
  taskId: string;
  statusId: string;
  projectId: string;
}

export interface TaskItem {
  id: string;
  projectId: string;
  parentTaskId?: string;
  title: string;
  description?: string;
  type: 'Standard' | 'Deadline';
  statusId: string;
  startDate?: string;
  endDate?: string;
  createdAt: string;
  assignees: TaskAssignee[];
  subTasks?: TaskItem[];
}

export interface TaskAssignee {
  taskId: string;
  userId: string;
  assignedAt: string;
}

export interface Member {
  userId: string;
  name: string;
  email: string;
  joinedAt: string;
  isCreator: boolean;
}

export interface ExpenseCategory {
  id: string;
  projectId: string;
  name: string;
}

export interface Expense {
  id: string;
  projectId: string;
  categoryId?: string;
  amount: number;
  description?: string;
  date: string;
  createdByUserId: string;
  createdAt: string;
  category?: ExpenseCategory;
}
