export interface ApiResponse<T = any> {
  success: boolean;
  code: string;
  data?: T;
  message?: string;
}

export interface ApiError {
  success: false;
  code: string;
  message?: string;
}