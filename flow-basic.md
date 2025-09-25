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