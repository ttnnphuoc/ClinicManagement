import { Form, Input, Button, Checkbox, Card, message } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate } from 'react-router-dom';
import { useState } from 'react';
import { authService } from '../services/authService';

const Login = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const onFinish = async (values: any) => {
    setLoading(true);
    try {
      const response = await authService.login({
        emailOrPhone: values.emailOrPhone,
        password: values.password,
      });
      
      if (response.success && response.data) {
        localStorage.setItem('token', response.data.token);
        localStorage.setItem('user', JSON.stringify({
          fullName: response.data.fullName,
          email: response.data.email,
          role: response.data.role,
        }));
        
        message.success(t(`success.${response.code}`) || t('auth.loginSuccess'));
        navigate('/');
      }
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      const errorMessage = t(`errors.${errorCode}`);
      message.error(errorMessage);
    } finally {
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
        <div style={{ textAlign: 'center', marginBottom: '30px' }}>
          <h1 style={{ fontSize: '28px', fontWeight: 'bold', color: '#1890ff', marginBottom: '8px' }}>
            {t('common.appName')}
          </h1>
          <h2 style={{ fontSize: '20px', fontWeight: 600, margin: '0 0 8px 0' }}>
            {t('auth.loginTitle')}
          </h2>
          <p style={{ color: '#8c8c8c', margin: 0 }}>
            {t('auth.loginSubtitle')}
          </p>
        </div>

        <Form
          name="login"
          onFinish={onFinish}
          size="large"
          autoComplete="off"
        >
          <Form.Item
            name="emailOrPhone"
            rules={[
              { required: true, message: `Please input your ${t('auth.emailOrPhone').toLowerCase()}!` }
            ]}
          >
            <Input 
              prefix={<UserOutlined />} 
              placeholder={t('auth.emailOrPhone')} 
            />
          </Form.Item>

          <Form.Item
            name="password"
            rules={[{ required: true, message: `Please input your ${t('auth.password').toLowerCase()}!` }]}
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder={t('auth.password')}
            />
          </Form.Item>

          <Form.Item>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <Form.Item name="remember" valuePropName="checked" noStyle>
                <Checkbox>{t('auth.rememberMe')}</Checkbox>
              </Form.Item>
              <Link to="/forgot-password" style={{ color: '#1890ff' }}>
                {t('auth.forgotPassword')}
              </Link>
            </div>
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" block loading={loading}>
              {t('auth.loginButton')}
            </Button>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
};

export default Login;