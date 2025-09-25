import React, { useState } from 'react';
import { Modal, Form, Input, Switch, Button, Steps, message } from 'antd';
import { useTranslation } from 'react-i18next';
import PackageSelection from './PackageSelection';
import { subscriptionService, type CreateClinicWithPackageRequest } from '../services/subscriptionService';

const { Step } = Steps;

interface CreateClinicWithPackageProps {
  open: boolean;
  onCancel: () => void;
  onSuccess: () => void;
}

const CreateClinicWithPackage: React.FC<CreateClinicWithPackageProps> = ({
  open,
  onCancel,
  onSuccess,
}) => {
  const { t } = useTranslation();
  const [form] = Form.useForm();
  const [currentStep, setCurrentStep] = useState(0);
  const [selectedPackage, setSelectedPackage] = useState<string>('');
  const [loading, setLoading] = useState(false);

  const handleNext = () => {
    if (currentStep === 0) {
      form
        .validateFields()
        .then(() => {
          setCurrentStep(1);
        })
        .catch(() => {
          message.error('Please fill in all required fields');
        });
    } else if (currentStep === 1) {
      if (!selectedPackage) {
        message.error('Please select a subscription package');
        return;
      }
      setCurrentStep(2);
    }
  };

  const handlePrev = () => {
    setCurrentStep(currentStep - 1);
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      if (!selectedPackage) {
        message.error('Please select a subscription package');
        return;
      }

      setLoading(true);
      const requestData: CreateClinicWithPackageRequest = {
        ...values,
        packageId: selectedPackage,
      };

      const response = await subscriptionService.createClinicWithPackage(requestData);
      if (response.success) {
        message.success('Clinic created successfully with subscription!');
        handleClose();
        onSuccess();
      } else {
        message.error('Failed to create clinic with subscription');
      }
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      message.error(`Error: ${errorCode}`);
    } finally {
      setLoading(false);
    }
  };

  const handleClose = () => {
    form.resetFields();
    setCurrentStep(0);
    setSelectedPackage('');
    onCancel();
  };

  const renderStepContent = () => {
    switch (currentStep) {
      case 0:
        return (
          <Form
            form={form}
            layout="vertical"
            initialValues={{ isActive: true }}
          >
            <Form.Item
              name="name"
              label={t('clinics.name')}
              rules={[{ required: true, message: t('clinics.nameRequired') }]}
            >
              <Input placeholder="Enter clinic name" />
            </Form.Item>

            <Form.Item
              name="address"
              label={t('clinics.address')}
              rules={[{ required: true, message: t('clinics.addressRequired') }]}
            >
              <Input.TextArea rows={3} placeholder="Enter clinic address" />
            </Form.Item>

            <Form.Item
              name="phoneNumber"
              label={t('clinics.phoneNumber')}
              rules={[{ required: true, message: t('clinics.phoneRequired') }]}
            >
              <Input placeholder="Enter phone number" />
            </Form.Item>

            <Form.Item
              name="email"
              label={t('clinics.email')}
              rules={[{ type: 'email', message: t('patients.emailInvalid') }]}
            >
              <Input placeholder="Enter email address (optional)" />
            </Form.Item>

            <Form.Item name="isActive" label={t('clinics.status')} valuePropName="checked">
              <Switch checkedChildren={t('clinics.active')} unCheckedChildren={t('clinics.inactive')} />
            </Form.Item>
          </Form>
        );
      case 1:
        return (
          <div>
            <h3 style={{ marginBottom: '20px', textAlign: 'center' }}>
              Choose Your Subscription Package
            </h3>
            <PackageSelection
              onPackageSelect={setSelectedPackage}
              selectedPackageId={selectedPackage}
              showSelectButton={false}
            />
          </div>
        );
      case 2:
        return (
          <div style={{ textAlign: 'center', padding: '40px 0' }}>
            <h3>Review Your Setup</h3>
            <div style={{ marginBottom: '24px', textAlign: 'left', maxWidth: '400px', margin: '0 auto' }}>
              <p><strong>Clinic Name:</strong> {form.getFieldValue('name')}</p>
              <p><strong>Address:</strong> {form.getFieldValue('address')}</p>
              <p><strong>Phone:</strong> {form.getFieldValue('phoneNumber')}</p>
              {form.getFieldValue('email') && (
                <p><strong>Email:</strong> {form.getFieldValue('email')}</p>
              )}
              <p><strong>Package:</strong> Selected package will be activated</p>
            </div>
            <p style={{ color: '#666', fontSize: '14px' }}>
              Click "Create Clinic" to complete the setup. Your subscription will be activated automatically.
            </p>
          </div>
        );
      default:
        return null;
    }
  };

  const getStepTitle = (step: number) => {
    switch (step) {
      case 0:
        return 'Clinic Information';
      case 1:
        return 'Choose Package';
      case 2:
        return 'Review & Create';
      default:
        return '';
    }
  };

  return (
    <Modal
      title="Create New Clinic with Subscription"
      open={open}
      onCancel={handleClose}
      footer={null}
      width={1000}
      destroyOnClose
    >
      <Steps current={currentStep} style={{ marginBottom: '32px' }}>
        {[0, 1, 2].map((step) => (
          <Step key={step} title={getStepTitle(step)} />
        ))}
      </Steps>

      <div style={{ minHeight: '400px' }}>
        {renderStepContent()}
      </div>

      <div style={{ marginTop: '24px', textAlign: 'right' }}>
        {currentStep > 0 && (
          <Button style={{ marginRight: '8px' }} onClick={handlePrev}>
            Previous
          </Button>
        )}
        {currentStep < 2 && (
          <Button type="primary" onClick={handleNext}>
            Next
          </Button>
        )}
        {currentStep === 2 && (
          <Button type="primary" onClick={handleSubmit} loading={loading}>
            Create Clinic
          </Button>
        )}
      </div>
    </Modal>
  );
};

export default CreateClinicWithPackage;