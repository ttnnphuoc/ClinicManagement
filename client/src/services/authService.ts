import api from './api';
import type { ApiResponse } from '../types/api';

export interface LoginRequest {
  emailOrPhone: string;
  password: string;
}

export interface ClinicInfo {
  id: string;
  name: string;
}

export interface LoginResponse {
  token: string;
  fullName: string;
  email: string;
  role: string;
  clinics?: ClinicInfo[];
}

export interface SelectClinicRequest {
  emailOrPhone: string;
  password: string;
  clinicId: string;
}

export const authService = {
  login: async (data: LoginRequest): Promise<ApiResponse<LoginResponse>> => {
    const response = await api.post<ApiResponse<LoginResponse>>('/auth/login', data);
    return response.data;
  },

  selectClinic: async (data: SelectClinicRequest): Promise<ApiResponse<LoginResponse>> => {
    const response = await api.post<ApiResponse<LoginResponse>>('/auth/select-clinic', data);
    return response.data;
  },
};