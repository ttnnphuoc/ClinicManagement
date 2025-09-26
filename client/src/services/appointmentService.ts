import api from './api';
import type { ApiResponse } from '../types/api';

export interface AppointmentService {
  id: string;
  serviceId: string;
  serviceName: string;
  price: number;
  duration: number;
}

export interface Appointment {
  id: string;
  clinicId: string;
  patientId: string;
  patientName: string;
  staffId: string;
  staffName: string;
  appointmentDate: string;
  status: string;
  notes?: string;
  services?: AppointmentService[];
  createdAt: string;
}

export interface CreateAppointmentRequest {
  PatientId: string;
  StaffId?: string | null;
  AppointmentDate: string;
  Status: string;
  Notes?: string;
  ServiceIds?: string[];
}

export interface UpdateAppointmentRequest {
  patientId: string;
  staffId: string;
  appointmentDate: string;
  status: string;
  notes?: string;
}

export interface UpdateAppointmentStatusRequest {
  status: string;
}

export interface AppointmentListResponse {
  items: Appointment[];
  total: number;
  page: number;
  pageSize: number;
}

export const appointmentService = {
  getAppointments: async (startDate?: string, endDate?: string): Promise<ApiResponse<Appointment[]>> => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);
    
    const response = await api.get<ApiResponse<Appointment[]>>(`/appointments?${params}`);
    return response.data;
  },

  getAppointment: async (id: string): Promise<ApiResponse<Appointment>> => {
    const response = await api.get<ApiResponse<Appointment>>(`/appointments/${id}`);
    return response.data;
  },

  createAppointment: async (data: CreateAppointmentRequest): Promise<ApiResponse<Appointment>> => {
    const response = await api.post<ApiResponse<Appointment>>('/appointments', data);
    return response.data;
  },

  updateAppointment: async (id: string, data: UpdateAppointmentRequest): Promise<ApiResponse<Appointment>> => {
    const response = await api.put<ApiResponse<Appointment>>(`/appointments/${id}`, data);
    return response.data;
  },

  updateAppointmentStatus: async (id: string, status: string): Promise<ApiResponse<Appointment>> => {
    const response = await api.patch<ApiResponse<Appointment>>(`/appointments/${id}/status`, { status });
    return response.data;
  },

  deleteAppointment: async (id: string): Promise<ApiResponse<void>> => {
    const response = await api.delete<ApiResponse<void>>(`/appointments/${id}`);
    return response.data;
  },
};