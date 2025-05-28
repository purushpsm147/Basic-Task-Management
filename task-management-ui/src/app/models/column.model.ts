export interface Column {
  id: number;
  name: string;
  description?: string;
  order: number;
  color?: string;
  isDefault: boolean;
  tasks?: JiraTask[];
}

import { JiraTask } from './jira-task.model';

export interface CreateColumnRequest {
  name: string;
  description?: string;
  order?: number;
  color?: string;
  isDefault?: boolean;
}

export interface UpdateColumnRequest {
  id: number;
  name: string;
  description?: string;
  order?: number;
  color?: string;
  isDefault?: boolean;
}
