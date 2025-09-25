// Role enum mapping - matches backend UserRole enum
export const UserRoleEnum = {
  SuperAdmin: 0,
  ClinicManager: 1,
  Doctor: 2,
  Nurse: 3,
  Receptionist: 4,
  Accountant: 5,
  Pharmacist: 6,
} as const;

// Reverse mapping for converting numeric values to string
const roleMap: { [key: number]: string } = {
  0: 'SuperAdmin',
  1: 'ClinicManager', 
  2: 'Doctor',
  3: 'Nurse',
  4: 'Receptionist',
  5: 'Accountant',
  6: 'Pharmacist'
};

export const getUserRole = (role: any): string => {
  if (typeof role === 'string') return role; // Already string format
  return roleMap[role] || '';
};

export const checkUserRole = (userRole: any, allowedRoles: string[]): boolean => {
  const role = getUserRole(userRole);
  return allowedRoles.includes(role);
};

export const isUserRole = (userRole: any, targetRole: string): boolean => {
  const role = getUserRole(userRole);
  return role === targetRole;
};