import { Form, Input, Button, DatePicker, InputNumber, Collapse, Row, Col, App } from 'antd';
import { useTranslation } from 'react-i18next';
import { useEffect } from 'react';
import dayjs from 'dayjs';
import { treatmentHistoryService } from '../services/treatmentHistoryService';
import type { TreatmentHistory, CreateTreatmentHistoryRequest } from '../services/treatmentHistoryService';

const { TextArea } = Input;

interface TreatmentFormProps {
  treatment: TreatmentHistory | null;
  patientId: string;
  appointmentId?: string;
  onSuccess: (treatment?: TreatmentHistory) => void;
  onCancel: () => void;
}

const TreatmentForm = ({ treatment, patientId, appointmentId, onSuccess, onCancel }: TreatmentFormProps) => {
  const { t } = useTranslation();
  const [form] = Form.useForm();
  const { message } = App.useApp();

  useEffect(() => {
    if (treatment) {
      form.setFieldsValue({
        ...treatment,
        treatmentDate: treatment.treatmentDate ? dayjs(treatment.treatmentDate) : null,
        nextAppointmentDate: treatment.nextAppointmentDate ? dayjs(treatment.nextAppointmentDate) : null,
      });
    } else {
      form.resetFields();
      form.setFieldsValue({
        treatmentDate: dayjs(),
        patientId,
        appointmentId
      });
    }
  }, [treatment, patientId, appointmentId, form]);

  const handleSubmit = async (values: any) => {
    try {
      const data: CreateTreatmentHistoryRequest = {
        patientId: values.patientId || patientId,
        appointmentId: values.appointmentId || appointmentId,
        treatmentDate: values.treatmentDate ? values.treatmentDate.toISOString() : dayjs().toISOString(),
        chiefComplaint: values.chiefComplaint,
        symptoms: values.symptoms,
        bloodPressure: values.bloodPressure,
        temperature: values.temperature,
        heartRate: values.heartRate,
        respiratoryRate: values.respiratoryRate,
        weight: values.weight,
        height: values.height,
        physicalExamination: values.physicalExamination,
        diagnosis: values.diagnosis,
        differentialDiagnosis: values.differentialDiagnosis,
        treatment: values.treatment,
        prescriptionNotes: values.prescriptionNotes,
        treatmentPlan: values.treatmentPlan,
        followUpInstructions: values.followUpInstructions,
        nextAppointmentDate: values.nextAppointmentDate ? values.nextAppointmentDate.toISOString() : undefined,
        notes: values.notes,
      };

      if (treatment) {
        const response = await treatmentHistoryService.updateTreatment(treatment.id, data);
        if (response.success) {
          message.success('Treatment updated successfully');
          onSuccess(response.data);
        }
      } else {
        const response = await treatmentHistoryService.createTreatment(data);
        if (response.success) {
          message.success('Treatment recorded successfully');
          onSuccess(response.data);
        }
      }
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      message.error(t(`errors.${errorCode}`));
    }
  };

  const items = [
    {
      key: '1',
      label: 'Chief Complaint & Symptoms',
      children: (
        <Row gutter={16}>
          <Col span={12}>
            <Form.Item name="chiefComplaint" label="Chief Complaint">
              <TextArea rows={3} placeholder="Patient's main concern or reason for visit..." />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="symptoms" label="Symptoms">
              <TextArea rows={3} placeholder="Current symptoms and their details..." />
            </Form.Item>
          </Col>
        </Row>
      ),
    },
    {
      key: '2',
      label: 'Vital Signs',
      children: (
        <Row gutter={16}>
          <Col span={8}>
            <Form.Item name="bloodPressure" label="Blood Pressure">
              <Input placeholder="120/80" />
            </Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item name="temperature" label="Temperature (°C)">
              <InputNumber style={{ width: '100%' }} placeholder="36.5" step={0.1} min={30} max={45} />
            </Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item name="heartRate" label="Heart Rate (bpm)">
              <InputNumber style={{ width: '100%' }} placeholder="72" min={30} max={200} />
            </Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item name="respiratoryRate" label="Respiratory Rate">
              <InputNumber style={{ width: '100%' }} placeholder="16" min={5} max={40} />
            </Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item name="weight" label="Weight (kg)">
              <InputNumber style={{ width: '100%' }} placeholder="70" step={0.1} min={1} max={300} />
            </Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item name="height" label="Height (cm)">
              <InputNumber style={{ width: '100%' }} placeholder="170" step={0.1} min={30} max={250} />
            </Form.Item>
          </Col>
        </Row>
      ),
    },
    {
      key: '3',
      label: 'Examination & Diagnosis',
      children: (
        <Row gutter={16}>
          <Col span={24}>
            <Form.Item name="physicalExamination" label="Physical Examination">
              <TextArea rows={4} placeholder="Physical examination findings..." />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="diagnosis" label="Primary Diagnosis">
              <TextArea rows={3} placeholder="Primary diagnosis..." />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="differentialDiagnosis" label="Differential Diagnosis">
              <TextArea rows={3} placeholder="Alternative diagnoses to consider..." />
            </Form.Item>
          </Col>
        </Row>
      ),
    },
    {
      key: '4',
      label: 'Treatment & Prescription',
      children: (
        <Row gutter={16}>
          <Col span={24}>
            <Form.Item
              name="treatment"
              label="Treatment Provided"
              rules={[{ required: true, message: 'Treatment is required' }]}
            >
              <TextArea rows={3} placeholder="Treatment provided during this visit..." />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="prescriptionNotes" label="Prescription Notes">
              <TextArea rows={4} placeholder="Medications prescribed..." />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="treatmentPlan" label="Treatment Plan">
              <TextArea rows={4} placeholder="Ongoing treatment plan..." />
            </Form.Item>
          </Col>
        </Row>
      ),
    },
    {
      key: '5',
      label: 'Follow-up & Notes',
      children: (
        <Row gutter={16}>
          <Col span={12}>
            <Form.Item name="followUpInstructions" label="Follow-up Instructions">
              <TextArea rows={3} placeholder="Instructions for patient..." />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="nextAppointmentDate" label="Next Appointment">
              <DatePicker showTime style={{ width: '100%' }} />
            </Form.Item>
          </Col>
          <Col span={24}>
            <Form.Item name="notes" label="Additional Notes">
              <TextArea rows={3} placeholder="Any additional notes..." />
            </Form.Item>
          </Col>
        </Row>
      ),
    },
  ];

  return (
    <Form
      form={form}
      layout="vertical"
      onFinish={handleSubmit}
    >
      <Row gutter={16}>
        <Col span={24}>
          <Form.Item
            name="treatmentDate"
            label="Treatment Date & Time"
            rules={[{ required: true, message: 'Treatment date is required' }]}
          >
            <DatePicker showTime style={{ width: '100%' }} />
          </Form.Item>
        </Col>
      </Row>

      <Collapse 
        items={items}
        defaultActiveKey={['1', '2', '3', '4', '5']}
        size="small"
      />

      <Form.Item style={{ marginTop: 16 }}>
        <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
          <Button onClick={onCancel}>
            {t('common.cancel')}
          </Button>
          <Button type="primary" htmlType="submit">
            {treatment ? 'Update Treatment' : 'Record Treatment'}
          </Button>
        </div>
      </Form.Item>
    </Form>
  );
};

export default TreatmentForm;