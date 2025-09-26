import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ConfigProvider, App as AntApp } from 'antd';
import MainLayout from './layouts/MainLayout';
import Dashboard from './pages/Dashboard';
import Login from './pages/Login';
import ForgotPassword from './pages/ForgotPassword';
import ClinicSelection from './pages/ClinicSelection';
import Patients from './pages/Patients';
import Appointments from './pages/Appointments';
import Services from './pages/Services';
import Medicines from './pages/Medicines';
import Clinics from './pages/Clinics';
import Staff from './pages/Staff';
import Subscription from './pages/Subscription';
import ProtectedRoute from './components/ProtectedRoute';
import './i18n/config';

function App() {
  return (
    <ConfigProvider
      theme={{
        token: {
          colorPrimary: '#1890ff',
        },
      }}
    >
      <AntApp>
        <BrowserRouter>
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/forgot-password" element={<ForgotPassword />} />
            <Route path="/select-clinic" element={<ClinicSelection />} />
            
            <Route path="/" element={<ProtectedRoute><MainLayout /></ProtectedRoute>}>
              <Route index element={<Dashboard />} />
              <Route path="patients" element={<Patients />} />
              <Route path="appointments" element={<Appointments />} />
              <Route path="services" element={<Services />} />
              <Route path="medicines" element={<Medicines />} />
              <Route path="clinics" element={<Clinics />} />
              <Route path="transactions" element={<div>Transactions Page</div>} />
              <Route path="inventory" element={<div>Inventory Page</div>} />
              <Route path="staff" element={<Staff />} />
              <Route path="subscription" element={<Subscription />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </AntApp>
    </ConfigProvider>
  );
}

export default App;