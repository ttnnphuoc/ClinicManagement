# Basic Clinic Management System Flows (MVP)

This document outlines the essential flows for a small-to-medium clinic management system. These flows are designed to address the core operational needs, ensuring smooth patient care, service delivery, and financial control.

---

## 1. Appointment Booking / Patient Reception

- Staff receives booking requests (phone, website, in person).
- Check doctor/room availability.
- Create appointment in the system:
  - Patient info (name, phone, code, DOB)
  - Time, doctor, requested service
  - Special notes (if any)
- Confirm appointment with the patient.

### Patient Check-in

- Patient arrives as scheduled.
- Staff verifies and creates/updates the patient profile.
- Mark patient as "waiting for consultation".

---

## 2. Medical Consultation

- Doctor calls patient into the exam room.
- Conducts examination, interviews, and provides advice.
- Records:
  - Reason for visit
  - Symptoms, preliminary diagnosis
  - Required services or clinical tests (lab, imaging, etc.)
  - Prescription (if any)
  - Consultation results are saved to the patient medical record.

---

## 3. Service Execution / Treatment

- Medical staff perform assigned services.
- Document results, upload images if required.
- Update service status (completed, pending, etc.)

---

## 4. Prescription & Medication Dispensing (if applicable)

### Prescription

- Doctor selects medicines, dosage, duration, and instructions.
- Save prescription to the system, link to the current visit.

### Medication Dispensing

- Pharmacy staff review prescription and dispense medications.
- Record dispensed quantities, update stock.
- Provide usage instructions to the patient.

---

## 5. Billing & Payment

- Cashier consolidates all services and dispensed meds for billing.
- Display itemized charges for patient review.
- Apply discounts, insurance, or vouchers (if applicable).

---

## 6. Receipt Issuance

- Cashier issues a receipt:
  - Receipt number, date, patient name, service/medication details, total, payment method
- Print or email the invoice/receipt to the patient.
- System marks the visit as paid.

---

## 7. Medical History Update

- All visit, service, medication, and payment history is saved to the patient profile.
- Staff/doctors can retrieve full patient history at any time.

---

## 8. Inventory & Medication Management (if applicable)

### Inventory In

- Record incoming medicines (quantity, expiry, supplier, batch, etc.)

### Inventory Out

- Automatic deduction when dispensing medications.
- Monitor current stock, alert for low or expiring inventory.

---

## 9. Follow-up Appointment Scheduling (if needed)

- Doctor or staff schedules next visit for the patient.
- Send appointment reminders via SMS/Zalo/email.

---

## 10. SaaS Subscription Management

### Clinic Registration & Package Selection

- New clinic owner registers account.
- System presents available subscription packages:
  - **Trial**: 14-day free (1 clinic, 50 patients, 2 staff, 100 appointments/month)
  - **Basic**: $29.99/month (1 clinic, 200 patients, 5 staff, unlimited appointments)
  - **Professional**: $79.99/month (3 clinics, 1,000 patients, 10 staff, unlimited)
  - **Premium**: $149.99/month (unlimited everything)
- Owner selects package and creates first clinic.
- Subscription activated automatically with usage tracking initialized.

### Usage Monitoring & Validation

- System tracks real-time usage across all resources:
  - Number of clinics created
  - Total patients registered
  - Staff members added
  - Monthly appointment count
- **Before** allowing new resource creation:
  - Middleware validates current usage against package limits
  - Blocks creation if limit exceeded
  - Shows upgrade suggestion
- **After** successful resource creation:
  - Updates usage counters
  - Sends warnings when approaching limits (75%, 90%)

### Subscription Management

- Clinic managers can view subscription status:
  - Current package details and pricing
  - Usage statistics with progress bars
  - Days until renewal/expiration
  - Payment history
- **Package Upgrades**:
  - Select higher-tier package
  - Prorated billing for immediate upgrade
  - Higher limits activated instantly
- **Cancellation**:
  - Disable auto-renewal (keeps service until end of billing period)
  - Data retention during grace period
- **Renewal**:
  - Automatic monthly/annual billing
  - Usage counters reset on renewal
  - Email notifications for payment issues

### Usage Limit Enforcement

- **Soft Limits** (Warning Phase):
  - Dashboard alerts at 75% usage
  - Email notifications to clinic managers
  - Upgrade prompts in UI
- **Hard Limits** (Blocking Phase):
  - API blocks new resource creation at 100%
  - Clear error messages with upgrade links
  - Existing resources remain functional
- **Grace Period**:
  - 7 days after subscription expires
  - Read-only access to data
  - Prominent renewal reminders

### Billing & Payment Processing

- **Subscription Billing**:
  - Automatic monthly/annual charges
  - Prorated billing for mid-cycle upgrades
  - Invoice generation with itemized breakdown
- **Payment Methods**:
  - Credit card integration (Stripe/PayPal)
  - Automatic retry for failed payments
  - Account suspension after 3 failed attempts
- **Financial Reporting**:
  - Revenue tracking per package tier
  - Customer lifetime value metrics
  - Churn analysis and retention reports

---

## 11. Multi-Tenant Data Isolation

### Clinic Context Management

- Each user session maintains current clinic context.
- Staff can belong to multiple clinics (with different roles).
- Data queries automatically filtered by clinic ID.
- Cross-clinic access prevented at middleware level.

### Role-Based Access Control

- **SuperAdmin**: Full system access, all clinics
- **ClinicManager**: Full access to assigned clinics only
- **Staff Roles**: Limited access based on role (Doctor, Nurse, Receptionist, etc.)
- Subscription management restricted to ClinicManager+ roles.

### Data Security & Privacy

- Patient data isolated per clinic
- Encrypted sensitive information
- Audit trails for all data access
- HIPAA/GDPR compliance measures