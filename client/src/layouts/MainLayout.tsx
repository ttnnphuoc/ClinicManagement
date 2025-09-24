import { Layout, Menu, Button } from 'antd';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import {
  DashboardOutlined,
  UserOutlined,
  CalendarOutlined,
  MedicineBoxOutlined,
  DollarOutlined,
  InboxOutlined,
  TeamOutlined,
  ShopOutlined,
} from '@ant-design/icons';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import LanguageSwitcher from '../components/LanguageSwitcher';

const { Header, Sider, Content } = Layout;

const MainLayout = () => {
  const [collapsed, setCollapsed] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const { t } = useTranslation();

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('currentClinic');
    navigate('/login');
  };

  const menuItems = [
    {
      key: '/',
      icon: <DashboardOutlined />,
      label: t('menu.dashboard'),
    },
    {
      key: '/patients',
      icon: <UserOutlined />,
      label: t('menu.patients'),
    },
    {
      key: '/appointments',
      icon: <CalendarOutlined />,
      label: t('menu.appointments'),
    },
    {
      key: '/services',
      icon: <MedicineBoxOutlined />,
      label: t('menu.services'),
    },
    {
      key: '/clinics',
      icon: <ShopOutlined />,
      label: t('menu.clinics'),
    },
    {
      key: '/transactions',
      icon: <DollarOutlined />,
      label: t('menu.transactions'),
    },
    {
      key: '/inventory',
      icon: <InboxOutlined />,
      label: t('menu.inventory'),
    },
    {
      key: '/staff',
      icon: <TeamOutlined />,
      label: t('menu.staff'),
    },
  ];

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider
        collapsible
        collapsed={collapsed}
        onCollapse={setCollapsed}
        breakpoint="lg"
        style={{ background: '#001529' }}
        width={250}
      >
        <div style={{ 
          height: '64px', 
          display: 'flex', 
          alignItems: 'center', 
          justifyContent: 'center',
          color: 'white',
          fontSize: collapsed ? '16px' : '18px',
          fontWeight: 'bold',
          padding: '0 16px'
        }}>
          {collapsed ? 'CM' : t('common.appName')}
        </div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[location.pathname]}
          items={menuItems}
          onClick={({ key }) => navigate(key)}
        />
      </Sider>
      <Layout>
        <Header style={{ 
          background: '#fff', 
          padding: '0 24px',
          boxShadow: '0 1px 4px rgba(0,21,41,.08)',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between'
        }}>
          <h1 style={{ margin: 0, fontSize: '20px', fontWeight: 600 }}>
            {t('common.appTitle')}
          </h1>
          <div style={{ display: 'flex', gap: '12px', alignItems: 'center' }}>
            <LanguageSwitcher />
            <Button type="text" danger onClick={handleLogout}>
              {t('auth.logout')}
            </Button>
          </div>
        </Header>
        <Content style={{ margin: '24px 16px', padding: 24, background: '#fff', borderRadius: '8px' }}>
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
};

export default MainLayout;