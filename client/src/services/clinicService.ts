import api from './api';
import type { ApiResponse } from '../types/api';

export interface Clinic {
  id: string;
  name: string;
  address: string;
  phoneNumber: string;
  email?: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateClinicRequest {
  name: string;
  address: string;
  phoneNumber: string;
  email?: string;
  isActive?: boolean;
}

export interface ClinicsListResponse {
  items: Clinic[];
  total: number;
  page: number;
  pageSize: number;
}

export const clinicService = {
  getClinics: async (search?: string, page = 1, pageSize = 10): Promise<ApiResponse<ClinicsListResponse>> => {
    const params = new URLSearchParams();
    if (search) params.append('search', search);
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    
    const response = await api.get<ApiResponse<ClinicsListResponse>>(`/clinics?${params}`);
    return response.data;
  },

  getActiveClinics: async (): Promise<ApiResponse<Clinic[]>> => {
    const response = await api.get<ApiResponse<Clinic[]>>('/clinics/active');
    return response.data;
  },

  getClinic: async (id: string): Promise<ApiResponse<Clinic>> => {
    const response = await api.get<ApiResponse<Clinic>>(`/clinics/${id}`);
    return response.data;
  },

  createClinic: async (data: CreateClinicRequest): Promise<ApiResponse<Clinic>> => {
    const response = await api.post<ApiResponse<Clinic>>('/clinics', data);
    return response.data;
  },

  updateClinic: async (id: string, data: CreateClinicRequest): Promise<ApiResponse<Clinic>> => {
    const response = await api.put<ApiResponse<Clinic>>(`/clinics/${id}`, data);
    return response.data;
  },

  deleteClinic: async (id: string): Promise<ApiResponse<void>> => {
    const response = await api.delete<ApiResponse<void>>(`/clinics/${id}`);
    return response.data;
  },

  getMyClinics: async (): Promise<ApiResponse<Clinic[]>> => {
    const response = await api.get<ApiResponse<Clinic[]>>('/clinics/my-clinics');
    return response.data;
  },
};