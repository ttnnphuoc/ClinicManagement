import { Form, Input, Button, DatePicker, Select, message, Collapse, Checkbox } from 'antd';
import { useTranslation } from 'react-i18next';
import { useEffect } from 'react';
import dayjs from 'dayjs';
import { patientService, Patient, CreatePatientRequest } from '../services/patientService';

interface PatientFormProps {
  patient: Patient | null;
  onSuccess: () => void;
  onCancel: () => void;
}

const PatientForm = ({ patient, onSuccess, onCancel }: PatientFormProps) => {
  const { t } = useTranslation();
  const [form] = Form.useForm();

  useEffect(() => {
    if (patient) {
      form.setFieldsValue({
        ...patient,
        dateOfBirth: patient.dateOfBirth ? dayjs(patient.dateOfBirth) : null,
      });
    } else {
      form.resetFields();
    }
  }, [patient, form]);

  const handleSubmit = async (values: any) => {
    try {
      const data: CreatePatientRequest = {
        fullName: values.fullName,
        phoneNumber: values.phoneNumber,
        email: values.email,
        dateOfBirth: values.dateOfBirth ? values.dateOfBirth.toISOString() : undefined,
        address: values.address,
        gender: values.gender,
        allergies: values.allergies,
        chronicConditions: values.chronicConditions,
        emergencyContactName: values.emergencyContactName,
        emergencyContactPhone: values.emergencyContactPhone,
        bloodType: values.bloodType,
        idNumber: values.idNumber,
        insuranceNumber: values.insuranceNumber,
        insuranceProvider: values.insuranceProvider,
        occupation: values.occupation,
        referralSource: values.referralSource,
        receivePromotions: values.receivePromotions || false,
        notes: values.notes,
      };

      if (patient) {
        const response = await patientService.updatePatient(patient.id, data);
        if (response.success) {
          message.success(t('patients.updateSuccess'));
          onSuccess();
        }
      } else {
        const response = await patientService.createPatient(data);
        if (response.success) {
          message.success(t('patients.createSuccess'));
          onSuccess();
        }
      }
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      message.error(t(`errors.${errorCode}`));
    }
  };

  return (
    <Form
      form={form}
      layout="vertical"
      onFinish={handleSubmit}
    >
      <Form.Item
        name="fullName"
        label={t('patients.fullName')}
        rules={[{ required: true, message: t('patients.fullNameRequired') }]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        name="phoneNumber"
        label={t('patients.phoneNumber')}
        rules={[{ required: true, message: t('patients.phoneNumberRequired') }]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        name="email"
        label={t('patients.email')}
        rules={[{ type: 'email', message: t('patients.emailInvalid') }]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        name="dateOfBirth"
        label={t('patients.dateOfBirth')}
      >
        <DatePicker style={{ width: '100%' }} />
      </Form.Item>

      <Form.Item
        name="gender"
        label={t('patients.gender')}
      >
        <Select>
          <Select.Option value="Male">{t('patients.male')}</Select.Option>
          <Select.Option value="Female">{t('patients.female')}</Select.Option>
          <Select.Option value="Other">{t('patients.other')}</Select.Option>
        </Select>
      </Form.Item>

      <Form.Item
        name="address"
        label={t('patients.address')}
      >
        <Input.TextArea rows={2} />
      </Form.Item>

      <Collapse defaultActiveKey={['1']} items={[
        {
          key: '1',
          label: t('patients.medicalInfo'),
          children: (
            <>
              <Form.Item name="allergies" label={t('patients.allergies')}>
                <Input.TextArea rows={2} />
              </Form.Item>
              <Form.Item name="chronicConditions" label={t('patients.chronicConditions')}>
                <Input.TextArea rows={2} />
              </Form.Item>
              <Form.Item name="emergencyContactName" label={t('patients.emergencyContactName')}>
                <Input />
              </Form.Item>
              <Form.Item name="emergencyContactPhone" label={t('patients.emergencyContactPhone')}>
                <Input />
              </Form.Item>
            </>
          ),
        },
        {
          key: '2',
          label: t('patients.additionalInfo'),
          children: (
            <>
              <Form.Item name="bloodType" label={t('patients.bloodType')}>
                <Select allowClear>
                  <Select.Option value="A+">A+</Select.Option>
                  <Select.Option value="A-">A-</Select.Option>
                  <Select.Option value="B+">B+</Select.Option>
                  <Select.Option value="B-">B-</Select.Option>
                  <Select.Option value="O+">O+</Select.Option>
                  <Select.Option value="O-">O-</Select.Option>
                  <Select.Option value="AB+">AB+</Select.Option>
                  <Select.Option value="AB-">AB-</Select.Option>
                </Select>
              </Form.Item>
              <Form.Item name="idNumber" label={t('patients.idNumber')}>
                <Input />
              </Form.Item>
              <Form.Item name="insuranceNumber" label={t('patients.insuranceNumber')}>
                <Input />
              </Form.Item>
              <Form.Item name="insuranceProvider" label={t('patients.insuranceProvider')}>
                <Input />
              </Form.Item>
              <Form.Item name="occupation" label={t('patients.occupation')}>
                <Input />
              </Form.Item>
              <Form.Item name="referralSource" label={t('patients.referralSource')}>
                <Input />
              </Form.Item>
              <Form.Item name="receivePromotions" valuePropName="checked">
                <Checkbox>{t('patients.receivePromotions')}</Checkbox>
              </Form.Item>
            </>
          ),
        },
      ]} />

      <Form.Item name="notes" label={t('patients.notes')}>
        <Input.TextArea rows={3} />
      </Form.Item>

      <Form.Item>
        <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
          <Button onClick={onCancel}>
            {t('common.cancel')}
          </Button>
          <Button type="primary" htmlType="submit">
            {t('common.save')}
          </Button>
        </div>
      </Form.Item>
    </Form>
  );
};

export default PatientForm;