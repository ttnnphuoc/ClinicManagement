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
  FileTextOutlined,
  UnorderedListOutlined,
  HomeOutlined,
  BellOutlined,
  FileDoneOutlined,
} from '@ant-design/icons';
import { useState, useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import LanguageSwitcher from '../components/LanguageSwitcher';

const { Header, Sider, Content } = Layout;

const MainLayout = () => {
  const [collapsed, setCollapsed] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const { t } = useTranslation();

  const user = JSON.parse(localStorage.getItem('user') || '{}');
  const isSuperAdmin = user.role === 'SuperAdmin';
  const canManageStaff = user.role === 'SuperAdmin' || user.role === 'ClinicManager';
  const canManageFinance = ['SuperAdmin', 'ClinicManager', 'Accountant'].includes(user.role);
  const canManageInventory = ['SuperAdmin', 'ClinicManager', 'Pharmacist'].includes(user.role);
  const canManageAppointments = ['SuperAdmin', 'ClinicManager', 'Doctor', 'Nurse', 'Receptionist'].includes(user.role);
  const canViewPatientRecords = ['SuperAdmin', 'ClinicManager', 'Doctor', 'Nurse'].includes(user.role);

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('currentClinic');
    navigate('/login');
  };

  const menuItems = useMemo(() => {
    const items = [
      {
        key: '/',
        icon: <DashboardOutlined />,
        label: t('menu.dashboard'),
      },
    ];

    // Patient Management
    if (canManageAppointments || canViewPatientRecords) {
      items.push({
        key: '/patients',
        icon: <UserOutlined />,
        label: t('menu.patients'),
      });
    }

    // Appointment & Queue Management
    if (canManageAppointments) {
      items.push(
        {
          key: '/appointments',
          icon: <CalendarOutlined />,
          label: t('menu.appointments'),
        },
        {
          key: '/queue',
          icon: <UnorderedListOutlined />,
          label: t('menu.queue'),
        }
      );
    }

    // Room Management
    if (canManageStaff) {
      items.push({
        key: '/rooms',
        icon: <HomeOutlined />,
        label: t('menu.rooms'),
      });
    }

    // Services
    items.push({
      key: '/services',
      icon: <MedicineBoxOutlined />,
      label: t('menu.services'),
    });

    // Medicine & Prescription Management
    if (canManageInventory || canViewPatientRecords) {
      items.push({
        key: '/medicines',
        icon: <MedicineBoxOutlined />,
        label: t('menu.medicines'),
      });

      if (canViewPatientRecords) {
        items.push({
          key: '/prescriptions',
          icon: <FileTextOutlined />,
          label: t('menu.prescriptions'),
        });
      }
    }

    // Financial Management
    if (canManageFinance) {
      items.push(
        {
          key: '/bills',
          icon: <DollarOutlined />,
          label: t('menu.bills'),
        },
        {
          key: '/receipts',
          icon: <FileDoneOutlined />,
          label: t('menu.receipts'),
        },
        {
          key: '/transactions',
          icon: <DollarOutlined />,
          label: t('menu.transactions'),
        }
      );
    }

    // Inventory Management
    if (canManageInventory) {
      items.push({
        key: '/inventory',
        icon: <InboxOutlined />,
        label: t('menu.inventory'),
      });
    }

    // Notifications
    items.push({
      key: '/notifications',
      icon: <BellOutlined />,
      label: t('menu.notifications'),
    });

    // Clinics (SuperAdmin only)
    if (isSuperAdmin) {
      items.push({
        key: '/clinics',
        icon: <ShopOutlined />,
        label: t('menu.clinics'),
      });
    }

    // Staff Management
    if (canManageStaff) {
      items.push({
        key: '/staff',
        icon: <TeamOutlined />,
        label: t('menu.staff'),
      });
    }

    return items;
  }, [t, isSuperAdmin, canManageStaff, canManageFinance, canManageInventory, canManageAppointments, canViewPatientRecords]);

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