import api from './api';
import type { ApiResponse } from '../types/api';

export interface Patient {
  id: string;
  clinicId: string;
  patientCode: string;
  fullName: string;
  phoneNumber: string;
  email?: string;
  dateOfBirth?: string;
  address?: string;
  gender?: string;
  allergies?: string;
  chronicConditions?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  bloodType?: string;
  idNumber?: string;
  insuranceNumber?: string;
  insuranceProvider?: string;
  occupation?: string;
  referralSource?: string;
  firstVisitDate?: string;
  receivePromotions: boolean;
  notes?: string;
  createdAt: string;
}

export interface CreatePatientRequest {
  fullName: string;
  phoneNumber: string;
  email?: string;
  dateOfBirth?: string;
  address?: string;
  gender?: string;
  allergies?: string;
  chronicConditions?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  bloodType?: string;
  idNumber?: string;
  insuranceNumber?: string;
  insuranceProvider?: string;
  occupation?: string;
  referralSource?: string;
  receivePromotions?: boolean;
  notes?: string;
}

export interface PatientsListResponse {
  items: Patient[];
  total: number;
  page: number;
  pageSize: number;
}

export const patientService = {
  getPatients: async (search?: string, page = 1, pageSize = 10): Promise<ApiResponse<PatientsListResponse>> => {
    const params = new URLSearchParams();
    if (search) params.append('search', search);
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    
    const response = await api.get<ApiResponse<PatientsListResponse>>(`/patients?${params}`);
    return response.data;
  },

  getPatient: async (id: string): Promise<ApiResponse<Patient>> => {
    const response = await api.get<ApiResponse<Patient>>(`/patients/${id}`);
    return response.data;
  },

  createPatient: async (data: CreatePatientRequest): Promise<ApiResponse<Patient>> => {
    const response = await api.post<ApiResponse<Patient>>('/patients', data);
    return response.data;
  },

  updatePatient: async (id: string, data: CreatePatientRequest): Promise<ApiResponse<Patient>> => {
    const response = await api.put<ApiResponse<Patient>>(`/patients/${id}`, data);
    return response.data;
  },

  deletePatient: async (id: string): Promise<ApiResponse<void>> => {
    const response = await api.delete<ApiResponse<void>>(`/patients/${id}`);
    return response.data;
  },
};