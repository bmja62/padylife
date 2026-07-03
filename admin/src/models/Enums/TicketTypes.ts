export type TicketTypes =
  | 'TechnicalIssue'
  | 'AccidentAnnouncement'
  | 'Other'
  | 'Financial'
  | 'ViolationReport'
  | 'Suggestion'
  | 'InsurancePaperIssue'

export type TicketResponseTypes =
  | 'UserResponse'
  | 'SupportResponse'
  | 'SystemResponse'

export type TicketStatuses = 'WaitingForSupport' | 'WaitingForUser' | 'Closed'

export enum TicketStatusesColor {
  WaitingForSupport = 'primary',
  WaitingForUser = 'warning',
  Closed = 'error',
}
export enum TicketStatusesPersian {
  WaitingForSupport = 'در انتظار پاسخ پشتیبانی',
  WaitingForUser = 'در انتظار پاسخ مشتری',
  Closed = 'بسته شده',
}

export enum TicketTypesPersian {
  TechnicalIssue = 'مشکل فنی',
  AccidentAnnouncement = 'اعلام تصادف',
  Other = 'سایر',
  Financial = 'مالی',
  ViolationReport = 'گزارش تخلف',
  Suggestion = 'پیشنهادات',
  InsurancePaperIssue = 'مشکل در پکیج نامه',
}

export enum TicketTypesColors {
  TechnicalIssue = 'error',
  AccidentAnnouncement = 'red-lighten-1',
  Other = 'teal-darken-1',
  Financial = 'deep-orange-darken-1',
  ViolationReport = 'deep-orange-accent-4',
  Suggestion = 'primary',
  InsurancePaperIssue = 'green-darken-4',
}
