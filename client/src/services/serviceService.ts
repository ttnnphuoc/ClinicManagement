import api from './api';
import type { ApiResponse } from '../types/api';

export interface Service {
  id: string;
  clinicId: string;
  name: string;
  description?: string;
  price: number;
  durationMinutes: number;
  isActive: boolean;
  createdAt: string;
}

export interface CreateServiceRequest {
  name: string;
  description?: string;
  price: number;
  durationMinutes: number;
  isActive?: boolean;
}

export interface ServicesListResponse {
  items: Service[];
  total: number;
  page: number;
  pageSize: number;
}

export const serviceService = {
  getServices: async (search?: string, page = 1, pageSize = 10): Promise<ApiResponse<ServicesListResponse>> => {
    const params = new URLSearchParams();
    if (search) params.append('search', search);
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    
    const response = await api.get<ApiResponse<ServicesListResponse>>(`/services?${params}`);
    return response.data;
  },

  getActiveServices: async (): Promise<ApiResponse<Service[]>> => {
    const response = await api.get<ApiResponse<Service[]>>('/services/active');
    return response.data;
  },

  getService: async (id: string): Promise<ApiResponse<Service>> => {
    const response = await api.get<ApiResponse<Service>>(`/services/${id}`);
    return response.data;
  },

  createService: async (data: CreateServiceRequest): Promise<ApiResponse<Service>> => {
    const response = await api.post<ApiResponse<Service>>('/services', data);
    return response.data;
  },

  updateService: async (id: string, data: CreateServiceRequest): Promise<ApiResponse<Service>> => {
    const response = await api.put<ApiResponse<Service>>(`/services/${id}`, data);
    return response.data;
  },

  deleteService: async (id: string): Promise<ApiResponse<void>> => {
    const response = await api.delete<ApiResponse<void>>(`/services/${id}`);
    return response.data;
  },
};