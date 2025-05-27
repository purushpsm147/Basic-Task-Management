export interface JiraTask {
  id: number;
  name: string;
  description: string;
  deadline: Date;
  isFavorite: boolean;
  status: JiraTaskStatus;
  imageURL?: string;
}

export enum JiraTaskStatus {
  Unassigned = 0,
  Approved = 1,
  InProgress = 2,
  Done = 3
}

export enum SortDirection {
  Asc = 0,
  Desc = 1
}

export interface CreateTaskRequest {
  name: string;
  description: string;
  deadline: Date;
  isFavorite: boolean;
  status: JiraTaskStatus;
}

export interface UpdateTaskRequest {
  id: number;
  name: string;
  description: string;
  deadline: Date;
  isFavorite: boolean;
  status: JiraTaskStatus;
  imageURL?: string;
}
