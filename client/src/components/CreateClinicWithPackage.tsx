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
          message.error(t('clinics.fillAllRequiredFields'));
        });
    } else if (currentStep === 1) {
      if (!selectedPackage) {
        message.error(t('clinics.selectSubscriptionPackage'));
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
        message.error(t('clinics.selectSubscriptionPackage'));
        return;
      }

      setLoading(true);
      const requestData: CreateClinicWithPackageRequest = {
        ...values,
        packageId: selectedPackage,
      };

      const response = await subscriptionService.createClinicWithPackage(requestData);
      if (response.success) {
        message.success(t('clinics.clinicCreatedWithSubscription'));
        handleClose();
        onSuccess();
      } else {
        message.error(t('clinics.failedToCreateClinicWithSubscription'));
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
              <Input placeholder={t('clinics.enterClinicName')} />
            </Form.Item>

            <Form.Item
              name="address"
              label={t('clinics.address')}
              rules={[{ required: true, message: t('clinics.addressRequired') }]}
            >
              <Input.TextArea rows={3} placeholder={t('clinics.enterClinicAddress')} />
            </Form.Item>

            <Form.Item
              name="phoneNumber"
              label={t('clinics.phoneNumber')}
              rules={[{ required: true, message: t('clinics.phoneRequired') }]}
            >
              <Input placeholder={t('clinics.enterPhoneNumber')} />
            </Form.Item>

            <Form.Item
              name="email"
              label={t('clinics.email')}
              rules={[{ type: 'email', message: t('patients.emailInvalid') }]}
            >
              <Input placeholder={t('clinics.enterEmailOptional')} />
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
              {t('clinics.chooseYourSubscriptionPackage')}
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
            <h3>{t('clinics.reviewYourSetup')}</h3>
            <div style={{ marginBottom: '24px', textAlign: 'left', maxWidth: '400px', margin: '0 auto' }}>
              <p><strong>{t('clinics.clinicName')}:</strong> {form.getFieldValue('name')}</p>
              <p><strong>{t('clinics.address')}:</strong> {form.getFieldValue('address')}</p>
              <p><strong>{t('clinics.phoneNumber')}:</strong> {form.getFieldValue('phoneNumber')}</p>
              {form.getFieldValue('email') && (
                <p><strong>{t('clinics.email')}:</strong> {form.getFieldValue('email')}</p>
              )}
              <p><strong>{t('clinics.packageLabel')}:</strong> {t('clinics.selectedPackageWillBeActivated')}</p>
            </div>
            <p style={{ color: '#666', fontSize: '14px' }}>
              {t('clinics.clickCreateClinicToComplete')}
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
        return t('clinics.clinicInformation');
      case 1:
        return t('clinics.choosePackage');
      case 2:
        return t('clinics.reviewAndCreate');
      default:
        return '';
    }
  };

  return (
    <Modal
      title={t('clinics.createNewClinicWithSubscription')}
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
            {t('clinics.previous')}
          </Button>
        )}
        {currentStep < 2 && (
          <Button type="primary" onClick={handleNext}>
            {t('clinics.next')}
          </Button>
        )}
        {currentStep === 2 && (
          <Button type="primary" onClick={handleSubmit} loading={loading}>
            {t('clinics.createClinic')}
          </Button>
        )}
      </div>
    </Modal>
  );
};

export default CreateClinicWithPackage;