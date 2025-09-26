import api from './api';
import type { ApiResponse } from '../types/api';

export interface Medicine {
  id: string;
  name: string;
  genericName?: string;
  manufacturer?: string;
  dosage?: string;
  form?: string; // Tablet, Capsule, Syrup, Injection, etc.
  price: number;
  description?: string;
  isActive: boolean;
  totalStock: number;
  createdAt: string;
}

export interface CreateMedicineRequest {
  name: string;
  genericName?: string;
  manufacturer?: string;
  dosage?: string;
  form?: string;
  price: number;
  description?: string;
  isActive?: boolean;
}

export interface UpdateMedicineRequest {
  name: string;
  genericName?: string;
  manufacturer?: string;
  dosage?: string;
  form?: string;
  price: number;
  description?: string;
  isActive: boolean;
}

export const medicineService = {
  getMedicines: async (): Promise<ApiResponse<Medicine[]>> => {
    const response = await api.get<ApiResponse<Medicine[]>>('/medicines');
    return response.data;
  },

  getMedicine: async (id: string): Promise<ApiResponse<Medicine>> => {
    const response = await api.get<ApiResponse<Medicine>>(`/medicines/${id}`);
    return response.data;
  },

  searchMedicines: async (searchTerm: string): Promise<ApiResponse<Medicine[]>> => {
    const response = await api.get<ApiResponse<Medicine[]>>(`/medicines/search?searchTerm=${encodeURIComponent(searchTerm)}`);
    return response.data;
  },

  createMedicine: async (data: CreateMedicineRequest): Promise<ApiResponse<Medicine>> => {
    const response = await api.post<ApiResponse<Medicine>>('/medicines', data);
    return response.data;
  },

  updateMedicine: async (id: string, data: UpdateMedicineRequest): Promise<ApiResponse<Medicine>> => {
    const response = await api.put<ApiResponse<Medicine>>(`/medicines/${id}`, data);
    return response.data;
  },

  deleteMedicine: async (id: string): Promise<ApiResponse<void>> => {
    const response = await api.delete<ApiResponse<void>>(`/medicines/${id}`);
    return response.data;
  },
};