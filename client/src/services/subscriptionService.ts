import api from './api';
import type { ApiResponse } from '../types/api';

export interface SubscriptionPackage {
  id: string;
  name: string;
  description: string;
  price: number;
  durationInDays: number;
  isTrialPackage: boolean;
  packageLimits: PackageLimit[];
}

export interface PackageLimit {
  limitType: string;
  limitValue: number;
  displayText: string;
}

export interface Subscription {
  id: string;
  userId: string;
  subscriptionPackage: SubscriptionPackage;
  startDate: string;
  endDate: string;
  status: string;
  autoRenew: boolean;
  lastPaymentDate?: string;
  usageTrackings: UsageTracking[];
}

export interface UsageTracking {
  resourceType: string;
  currentUsage: number;
  limit?: number;
  lastUpdated: string;
}

export interface CreateSubscriptionRequest {
  packageId: string;
}

export interface UpgradeSubscriptionRequest {
  newPackageId: string;
}

export interface CreateClinicWithPackageRequest {
  name: string;
  address: string;
  phoneNumber: string;
  email?: string;
  isActive: boolean;
  packageId: string;
}

export const subscriptionService = {
  // Get available packages
  getPackages: async (): Promise<ApiResponse<SubscriptionPackage[]>> => {
    const response = await api.get('/subscription/packages');
    return response.data;
  },

  // Get current subscription
  getCurrentSubscription: async (): Promise<ApiResponse<Subscription>> => {
    const response = await api.get('/subscription/current');
    return response.data;
  },

  // Subscribe to a package
  subscribe: async (data: CreateSubscriptionRequest): Promise<ApiResponse<Subscription>> => {
    const response = await api.post('/subscription/subscribe', data);
    return response.data;
  },

  // Upgrade subscription
  upgrade: async (data: UpgradeSubscriptionRequest): Promise<ApiResponse<Subscription>> => {
    const response = await api.put('/subscription/upgrade', data);
    return response.data;
  },

  // Cancel subscription
  cancel: async (): Promise<ApiResponse<void>> => {
    const response = await api.delete('/subscription/cancel');
    return response.data;
  },

  // Get usage tracking
  getUsage: async (): Promise<ApiResponse<UsageTracking[]>> => {
    const response = await api.get('/subscription/usage');
    return response.data;
  },

  // Create clinic with package
  createClinicWithPackage: async (data: CreateClinicWithPackageRequest): Promise<ApiResponse<any>> => {
    const response = await api.post('/clinics/with-package', data);
    return response.data;
  },
};