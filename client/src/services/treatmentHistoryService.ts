import api from './api';
import type { ApiResponse } from '../types/api';

export interface TreatmentHistory {
  id: string;
  clinicId: string;
  patientId: string;
  patientName: string;
  appointmentId?: string;
  staffId: string;
  staffName: string;
  treatmentDate: string;
  chiefComplaint?: string;
  symptoms?: string;
  bloodPressure?: string;
  temperature?: number;
  heartRate?: number;
  respiratoryRate?: number;
  weight?: number;
  height?: number;
  physicalExamination?: string;
  diagnosis?: string;
  differentialDiagnosis?: string;
  treatment: string;
  prescriptionNotes?: string;
  treatmentPlan?: string;
  followUpInstructions?: string;
  nextAppointmentDate?: string;
  notes?: string;
  createdAt: string;
}

export interface CreateTreatmentHistoryRequest {
  patientId: string;
  appointmentId?: string;
  treatmentDate: string;
  chiefComplaint?: string;
  symptoms?: string;
  bloodPressure?: string;
  temperature?: number;
  heartRate?: number;
  respiratoryRate?: number;
  weight?: number;
  height?: number;
  physicalExamination?: string;
  diagnosis?: string;
  differentialDiagnosis?: string;
  treatment: string;
  prescriptionNotes?: string;
  treatmentPlan?: string;
  followUpInstructions?: string;
  nextAppointmentDate?: string;
  notes?: string;
}

export interface UpdateTreatmentHistoryRequest {
  chiefComplaint?: string;
  symptoms?: string;
  bloodPressure?: string;
  temperature?: number;
  heartRate?: number;
  respiratoryRate?: number;
  weight?: number;
  height?: number;
  physicalExamination?: string;
  diagnosis?: string;
  differentialDiagnosis?: string;
  treatment: string;
  prescriptionNotes?: string;
  treatmentPlan?: string;
  followUpInstructions?: string;
  nextAppointmentDate?: string;
  notes?: string;
}

export interface TreatmentHistoryListResponse {
  items: TreatmentHistory[];
  total: number;
  page: number;
  pageSize: number;
}

export const treatmentHistoryService = {
  getTreatmentHistory: async (patientId?: string, search?: string, page = 1, pageSize = 10): Promise<ApiResponse<TreatmentHistoryListResponse>> => {
    const params = new URLSearchParams();
    if (patientId) params.append('patientId', patientId);
    if (search) params.append('search', search);
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    
    const response = await api.get<ApiResponse<TreatmentHistoryListResponse>>(`/treatmenthistory?${params}`);
    return response.data;
  },

  getPatientTreatmentHistory: async (patientId: string): Promise<ApiResponse<TreatmentHistory[]>> => {
    const response = await api.get<ApiResponse<TreatmentHistory[]>>(`/treatmenthistory/patient/${patientId}`);
    return response.data;
  },

  getTreatmentByAppointment: async (appointmentId: string): Promise<ApiResponse<TreatmentHistory>> => {
    const response = await api.get<ApiResponse<TreatmentHistory>>(`/treatmenthistory/appointment/${appointmentId}`);
    return response.data;
  },

  getTreatment: async (id: string): Promise<ApiResponse<TreatmentHistory>> => {
    const response = await api.get<ApiResponse<TreatmentHistory>>(`/treatmenthistory/${id}`);
    return response.data;
  },

  createTreatment: async (data: CreateTreatmentHistoryRequest): Promise<ApiResponse<TreatmentHistory>> => {
    const response = await api.post<ApiResponse<TreatmentHistory>>('/treatmenthistory', data);
    return response.data;
  },

  updateTreatment: async (id: string, data: UpdateTreatmentHistoryRequest): Promise<ApiResponse<TreatmentHistory>> => {
    const response = await api.put<ApiResponse<TreatmentHistory>>(`/treatmenthistory/${id}`, data);
    return response.data;
  },

  deleteTreatment: async (id: string): Promise<ApiResponse<void>> => {
    const response = await api.delete<ApiResponse<void>>(`/treatmenthistory/${id}`);
    return response.data;
  },
};