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
import Prescriptions from './pages/Prescriptions';
import Clinics from './pages/Clinics';
import Staff from './pages/Staff';
import Subscription from './pages/Subscription';
import Queue from './pages/Queue';
// import Rooms from './pages/Rooms';
import Receipts from './pages/Receipts';
import Transactions from './pages/Transactions';
import Inventory from './pages/Inventory';
import Notifications from './pages/Notifications';
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
              <Route path="queue" element={<Queue />} />
              {/* <Route path="rooms" element={<Rooms />} /> */}
              <Route path="services" element={<Services />} />
              <Route path="medicines" element={<Medicines />} />
              <Route path="prescriptions" element={<Prescriptions />} />
              <Route path="receipts" element={<Receipts />} />
              <Route path="transactions" element={<Transactions />} />
              <Route path="inventory" element={<Inventory />} />
              <Route path="notifications" element={<Notifications />} />
              <Route path="clinics" element={<Clinics />} />
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