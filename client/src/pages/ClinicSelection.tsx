import { useEffect, useState } from 'react';
import { Card, Row, Col, Typography, Spin, message } from 'antd';
import { ShopOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { authService, ClinicInfo } from '../services/authService';

const { Title, Text } = Typography;

const ClinicSelection = () => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [clinics, setClinics] = useState<ClinicInfo[]>([]);
  const [loading, setLoading] = useState(false);
  const [user, setUser] = useState<any>(null);

  useEffect(() => {
    const availableClinics = localStorage.getItem('availableClinics');
    const tempUser = localStorage.getItem('tempUser');
    
    if (!availableClinics || !tempUser) {
      navigate('/login');
      return;
    }

    setClinics(JSON.parse(availableClinics));
    setUser(JSON.parse(tempUser));
  }, [navigate]);

  const handleSelectClinic = async (clinic: ClinicInfo) => {
    setLoading(true);
    try {
      const credentials = localStorage.getItem('tempCredentials');
      if (!credentials) {
        navigate('/login');
        return;
      }

      const { emailOrPhone, password } = JSON.parse(credentials);
      
      const response = await authService.selectClinic({
        emailOrPhone,
        password,
        clinicId: clinic.id,
      });

      if (response.success && response.data) {
        localStorage.setItem('token', response.data.token);
        localStorage.setItem('user', JSON.stringify({
          fullName: response.data.fullName,
          email: response.data.email,
          role: response.data.role,
        }));
        localStorage.setItem('currentClinic', JSON.stringify(clinic));
        
        localStorage.removeItem('tempCredentials');
        localStorage.removeItem('tempUser');
        localStorage.removeItem('availableClinics');

        message.success(t('auth.clinicSelected'));
        navigate('/');
      }
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      message.error(t(`errors.${errorCode}`));
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
      <div style={{ width: '100%', maxWidth: '900px' }}>
        <Card>
          <div style={{ textAlign: 'center', marginBottom: '30px' }}>
            <Title level={2}>{t('auth.selectClinicTitle')}</Title>
            {user && (
              <Text type="secondary">
                {t('auth.welcomeUser', { name: user.fullName })}
              </Text>
            )}
          </div>

          <Spin spinning={loading}>
            <Row gutter={[16, 16]}>
              {clinics.map((clinic) => (
                <Col xs={24} sm={12} md={8} key={clinic.id}>
                  <Card
                    hoverable
                    onClick={() => handleSelectClinic(clinic)}
                    style={{ 
                      textAlign: 'center',
                      cursor: 'pointer',
                      height: '150px',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center'
                    }}
                  >
                    <ShopOutlined style={{ fontSize: '48px', color: '#1890ff', marginBottom: '16px' }} />
                    <Title level={4} style={{ margin: 0 }}>{clinic.name}</Title>
                  </Card>
                </Col>
              ))}
            </Row>
          </Spin>
        </Card>
      </div>
    </div>
  );
};

export default ClinicSelection;