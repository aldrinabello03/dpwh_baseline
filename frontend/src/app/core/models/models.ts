// Core models for DPWH HRIS
export interface User {
  id: string;
  username: string;
  email: string;
  roleName?: string;
  officeName?: string;
  isActive: boolean;
  photoPath?: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  status?: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface AuthResult {
  success: boolean;
  token?: string;
  refreshToken?: string;
  message?: string;
  user?: User;
}

export interface Employee {
  id: string;
  employeeNumber: string;
  fullName: string;
  firstName: string;
  middleName?: string;
  lastName: string;
  suffix?: string;
  gender: string;
  dateOfBirth: string;
  civilStatus: string;
  employmentType: string;
  officeName?: string;
  positionTitle?: string;
  status: string;
  photoPath?: string;
  emailAddress?: string;
  contactNumber?: string;
}

export interface DashboardSummary {
  permanentCount: number;
  contractualCount: number;
  casualCount: number;
  jobOrderCount: number;
  totalEmployees: number;
}

export interface Announcement {
  id: string;
  title: string;
  content: string;
  imagePath?: string;
  startDate?: string;
  endDate?: string;
  postedBy?: string;
}

export interface Memorandum {
  id: string;
  title: string;
  referenceNumber?: string;
  dateIssued: string;
  filePath?: string;
}

export interface DownloadableForm {
  id: string;
  formName: string;
  description?: string;
  category?: string;
  filePath: string;
  version?: string;
}

export interface Notification {
  id: string;
  title: string;
  message: string;
  isRead: boolean;
  notificationType: string;
  createdDate: string;
  relatedEntityType?: string;
  relatedEntityId?: string;
}

export interface LeaveApplication {
  id: string;
  employeeFullName: string;
  leaveType: string;
  dateFrom: string;
  dateTo: string;
  totalDays: number;
  status: string;
  reason?: string;
  createdDate: string;
}
