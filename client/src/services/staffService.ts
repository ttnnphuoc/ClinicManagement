import api from './api';

export interface Staff {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  role: string;
  isActive: boolean;
  clinics: StaffClinic[];
  createdAt: string;
}

export interface StaffClinic {
  clinicId: string;
  clinicName: string;
  isActive: boolean;
}

export interface CreateStaffRequest {
  fullName: string;
  email: string;
  password: string;
  phoneNumber: string;
  role: string;
  clinicIds: string[];
  isActive: boolean;
}

export interface UpdateStaffRequest {
  fullName: string;
  email: string;
  phoneNumber: string;
  role: string;
  clinicIds: string[];
  isActive: boolean;
}

export interface StaffListResponse {
  items: Staff[];
  total: number;
  page: number;
  pageSize: number;
}

export const staffService = {
  getStaff: async (search?: string, page: number = 1, pageSize: number = 10, clinicId?: string | null) => {
    const params: any = { search, page, pageSize };
    if (clinicId) {
      params.clinicId = clinicId;
    }
    const response = await api.get<StaffListResponse>('/staff', {
      params,
    });
    return response.data;
  },

  getStaffById: async (id: string) => {
    const response = await api.get<Staff>(`/staff/${id}`);
    return response.data;
  },

  createStaff: async (data: CreateStaffRequest) => {
    const response = await api.post<Staff>('/staff', data);
    return response.data;
  },

  updateStaff: async (id: string, data: UpdateStaffRequest) => {
    const response = await api.put<Staff>(`/staff/${id}`, data);
    return response.data;
  },

  changePassword: async (id: string, newPassword: string) => {
    const response = await api.put(`/staff/${id}/password`, { newPassword });
    return response.data;
  },

  deleteStaff: async (id: string) => {
    const response = await api.delete(`/staff/${id}`);
    return response.data;
  },
};