import { Form, Input, Button, Card, message, Result } from 'antd';
import { MailOutlined, ArrowLeftOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { useState } from 'react';

const ForgotPassword = () => {
  const { t } = useTranslation();
  const [loading, setLoading] = useState(false);
  const [submitted, setSubmitted] = useState(false);

  const onFinish = async (values: any) => {
    setLoading(true);
    try {
      console.log('Reset password for:', values);
      
      setTimeout(() => {
        setLoading(false);
        setSubmitted(true);
        message.success(t('auth.resetLinkSent'));
      }, 1500);
    } catch (error) {
      message.error('Failed to send reset link. Please try again.');
      setLoading(false);
    }
  };

  return (
    <div style={{
      minHeight: '100vh',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
      padding: '20px'
    }}>
      <Card 
        style={{ 
          width: '100%', 
          maxWidth: '400px',
          boxShadow: '0 10px 40px rgba(0,0,0,0.1)'
        }}
      >
        {!submitted ? (
          <>
            <div style={{ textAlign: 'center', marginBottom: '30px' }}>
              <h1 style={{ fontSize: '28px', fontWeight: 'bold', color: '#1890ff', marginBottom: '8px' }}>
                {t('common.appName')}
              </h1>
              <h2 style={{ fontSize: '20px', fontWeight: 600, margin: '0 0 8px 0' }}>
                {t('auth.forgotPasswordTitle')}
              </h2>
              <p style={{ color: '#8c8c8c', margin: 0 }}>
                {t('auth.forgotPasswordSubtitle')}
              </p>
            </div>

            <Form
              name="forgot-password"
              onFinish={onFinish}
              size="large"
              autoComplete="off"
            >
              <Form.Item
                name="email"
                rules={[
                  { required: true, message: `Please input your ${t('auth.email').toLowerCase()}!` },
                  { type: 'email', message: 'Please enter a valid email address!' }
                ]}
              >
                <Input 
                  prefix={<MailOutlined />} 
                  placeholder={t('auth.email')} 
                  type="email"
                />
              </Form.Item>

              <Form.Item>
                <Button type="primary" htmlType="submit" block loading={loading}>
                  {t('auth.resetPassword')}
                </Button>
              </Form.Item>

              <Form.Item style={{ marginBottom: 0 }}>
                <Link to="/login" style={{ display: 'flex', alignItems: 'center', gap: '8px', color: '#1890ff' }}>
                  <ArrowLeftOutlined /> {t('auth.backToLogin')}
                </Link>
              </Form.Item>
            </Form>
          </>
        ) : (
          <Result
            status="success"
            title={t('auth.resetLinkSent')}
            subTitle={t('auth.checkEmail')}
            extra={[
              <Link to="/login" key="login">
                <Button type="primary">
                  {t('auth.backToLogin')}
                </Button>
              </Link>
            ]}
          />
        )}
      </Card>
    </div>
  );
};

export default ForgotPassword;