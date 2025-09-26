import { Calendar, Badge, Modal, Form, Input, DatePicker, Select, Button, App, AutoComplete, Typography, Space, Table, Tabs, Tag, Spin } from 'antd';
import { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { PlusOutlined, MedicineBoxOutlined } from '@ant-design/icons';
import type { Dayjs } from 'dayjs';
import dayjs from 'dayjs';
import { serviceService, type Service } from '../services/serviceService';
import { patientService, type Patient } from '../services/patientService';
import { treatmentHistoryService, type TreatmentHistory } from '../services/treatmentHistoryService';
import { appointmentService, type Appointment as ApiAppointment } from '../services/appointmentService';
import PatientForm from '../components/PatientForm';
import TreatmentForm from '../components/TreatmentForm';

const { Text } = Typography;

// Helper function to get appointment title and badge type from API data
const getAppointmentTitle = (appointment: ApiAppointment) => {
  return `${appointment.services?.map(s => s.serviceName).join(', ') || 'Appointment'} - ${appointment.patientName}`;
};

const getAppointmentBadgeType = (status: string): 'warning' | 'success' | 'error' | 'default' => {
  switch (status.toLowerCase()) {
    case 'scheduled': return 'warning';
    case 'completed': return 'success';
    case 'cancelled': return 'error';
    case 'in progress': return 'warning';
    default: return 'default';
  }
};

const Appointments = () => {
  const { t } = useTranslation();
  const { message } = App.useApp();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(null);
  const [services, setServices] = useState<Service[]>([]);
  const [patients, setPatients] = useState<Patient[]>([]);
  const [selectedPatient, setSelectedPatient] = useState<Patient | null>(null);
  const [patientSearchText, setPatientSearchText] = useState('');
  const [isPatientModalOpen, setIsPatientModalOpen] = useState(false);
  const [isTreatmentModalOpen, setIsTreatmentModalOpen] = useState(false);
  const [selectedAppointment, setSelectedAppointment] = useState<ApiAppointment | null>(null);
  const [existingTreatment, setExistingTreatment] = useState<TreatmentHistory | null>(null);
  const [activeTab, setActiveTab] = useState('calendar');
  const [appointments, setAppointments] = useState<ApiAppointment[]>([]);
  const [appointmentsLoading, setAppointmentsLoading] = useState(false);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchServices();
    fetchAppointments();
  }, []);

  const fetchAppointments = async (start?: Dayjs, end?: Dayjs) => {
    setAppointmentsLoading(true);
    try {
      // Fetch appointments for the specified range or current month by default
      const startDate = (start || dayjs().startOf('month')).format('YYYY-MM-DD');
      const endDate = (end || dayjs().endOf('month')).format('YYYY-MM-DD');
      
      const response = await appointmentService.getAppointments(startDate, endDate);
      if (response.success && response.data) {
        setAppointments(response.data);
      }
    } catch (error) {
      console.error('Error fetching appointments:', error);
      message.error('Failed to load appointments');
    } finally {
      setAppointmentsLoading(false);
    }
  };

  const onPanelChange = (value: Dayjs, mode: string) => {
    if (mode === 'month') {
      // Fetch appointments for the new month
      const startDate = value.startOf('month');
      const endDate = value.endOf('month');
      fetchAppointments(startDate, endDate);
    }
  };

  const fetchServices = async () => {
    try {
      const response = await serviceService.getActiveServices();
      if (response.success && response.data) {
        setServices(response.data);
      }
    } catch (error) {
      console.error('Error fetching services:', error);
    }
  };

  const searchPatients = async (searchText: string) => {
    if (!searchText) {
      setPatients([]);
      return;
    }
    try {
      const response = await patientService.getPatients(searchText, 1, 10);
      if (response.success && response.data) {
        setPatients(response.data.items);
      }
    } catch (error) {
      console.error('Error searching patients:', error);
    }
  };

  const handlePatientSearch = (value: string) => {
    setPatientSearchText(value);
    searchPatients(value);
  };

  const handlePatientSelect = (value: string) => {
    const patient = patients.find(p => p.id === value);
    setSelectedPatient(patient || null);
    form.setFieldsValue({ patientId: value });
  };


  const getListData = (value: Dayjs) => {
    const dateStr = value.format('YYYY-MM-DD');
    return appointments.filter(apt => dayjs(apt.appointmentDate).format('YYYY-MM-DD') === dateStr);
  };

  const dateCellRender = (value: Dayjs) => {
    const listData = getListData(value);
    return (
      <ul style={{ listStyle: 'none', padding: 0 }}>
        {listData.map((appointment) => (
          <li key={appointment.id}>
            <Badge 
              status={getAppointmentBadgeType(appointment.status)} 
              text={getAppointmentTitle(appointment)} 
            />
          </li>
        ))}
      </ul>
    );
  };

  const onSelect = (date: Dayjs) => {
    setSelectedDate(date);
    form.setFieldsValue({ date });
    setIsModalOpen(true);
  };

  const resetForm = () => {
    form.resetFields();
    setSelectedPatient(null);
    setPatientSearchText('');
    setPatients([]);
  };

  const handlePatientCreated = (newPatient?: Patient) => {
    if (newPatient) {
      // Auto-select the newly created patient
      setSelectedPatient(newPatient);
      setPatientSearchText(newPatient.fullName);
      form.setFieldsValue({ patientId: newPatient.id });
      
      // Close patient modal
      setIsPatientModalOpen(false);
    }
  };

  const handlePatientModalClose = () => {
    setIsPatientModalOpen(false);
  };

  const handleRecordTreatment = async (appointment: ApiAppointment) => {
    setSelectedAppointment(appointment);
    
    // Check if treatment already exists for this appointment
    try {
      const response = await treatmentHistoryService.getTreatmentByAppointment(appointment.id);
      if (response.success && response.data) {
        setExistingTreatment(response.data);
      } else {
        setExistingTreatment(null);
      }
    } catch (error) {
      setExistingTreatment(null);
    }
    
    setIsTreatmentModalOpen(true);
  };

  const handleTreatmentSuccess = async () => {
    if (selectedAppointment && selectedAppointment.status !== 'Completed') {
      try {
        await appointmentService.updateAppointmentStatus(selectedAppointment.id, 'Completed');
        
        setAppointments(prev => prev.map(apt => 
          apt.id === selectedAppointment.id 
            ? { ...apt, status: 'Completed' }
            : apt
        ));
      } catch (error) {
        console.error('Error updating appointment status:', error);
      }
    }
    
    setIsTreatmentModalOpen(false);
    setSelectedAppointment(null);
    setExistingTreatment(null);
    message.success('Treatment recorded successfully');
  };

  const handleTreatmentModalClose = () => {
    setIsTreatmentModalOpen(false);
    setSelectedAppointment(null);
    setExistingTreatment(null);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Scheduled': return 'blue';
      case 'Completed': return 'green';
      case 'Cancelled': return 'red';
      case 'In Progress': return 'orange';
      default: return 'default';
    }
  };

  const appointmentColumns = [
    {
      title: 'Date & Time',
      dataIndex: 'appointmentDate',
      key: 'appointmentDate',
      render: (date: string) => dayjs(date).format('MMM DD, YYYY HH:mm'),
      sorter: (a: ApiAppointment, b: ApiAppointment) => dayjs(a.appointmentDate).unix() - dayjs(b.appointmentDate).unix(),
    },
    {
      title: 'Patient',
      dataIndex: 'patientName',
      key: 'patientName',
    },
    {
      title: 'Staff',
      dataIndex: 'staffName',
      key: 'staffName',
    },
    {
      title: 'Services',
      dataIndex: 'services',
      key: 'services',
      render: (services: any[]) => services?.map(s => s.serviceName).join(', ') || '-',
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      render: (status: string) => (
        <Tag color={getStatusColor(status)}>{status}</Tag>
      ),
    },
    {
      title: 'Actions',
      key: 'actions',
      render: (_: any, appointment: ApiAppointment) => (
        <Space>
          <Button
            type="primary"
            size="small"
            icon={<MedicineBoxOutlined />}
            onClick={() => handleRecordTreatment(appointment)}
            disabled={appointment.status === 'Cancelled'}
          >
            {appointment.status === 'Completed' ? 'View Treatment' : 'Record Treatment'}
          </Button>
        </Space>
      ),
    },
  ];

  const handleSubmit = async (values: any) => {
    try {
      const appointmentData = {
        PatientId: values.patientId,
        StaffId: null, // Will be auto-assigned by backend to current user
        AppointmentDate: values.date.toISOString(),
        Status: values.status,
        Notes: values.notes,
        ServiceIds: values.serviceIds
      };

      const response = await appointmentService.createAppointment(appointmentData);
      if (response.success) {
        message.success(t('appointments.createSuccess'));
        setIsModalOpen(false);
        resetForm();
        fetchAppointments(); // Refresh appointments list
      } else {
        message.error('Failed to create appointment');
      }
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      message.error(t(`errors.${errorCode}`));
    }
  };

  const tabItems = [
    {
      key: 'calendar',
      label: 'Calendar View',
      children: (
        <Spin spinning={appointmentsLoading}>
          <Calendar 
            dateCellRender={dateCellRender}
            onSelect={onSelect}
            onPanelChange={onPanelChange}
          />
        </Spin>
      ),
    },
    {
      key: 'list',
      label: 'List View',
      children: (
        <Table
          columns={appointmentColumns}
          dataSource={appointments}
          rowKey="id"
          loading={appointmentsLoading}
          pagination={{
            pageSize: 10,
            showTotal: (total) => `Total ${total} appointments`,
          }}
        />
      ),
    },
  ];

  return (
    <div>
      <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h2 style={{ margin: 0 }}>{t('appointments.title')}</h2>
        <Button type="primary" onClick={() => {
          setIsModalOpen(true);
          resetForm();
        }}>
          {t('appointments.addAppointment')}
        </Button>
      </div>

      <Tabs
        activeKey={activeTab}
        onChange={setActiveTab}
        items={tabItems}
      />

      <Modal
        title={t('appointments.addAppointment')}
        open={isModalOpen}
        onCancel={() => {
          setIsModalOpen(false);
          resetForm();
        }}
        footer={null}
        width={600}
      >
        <Form
          form={form}
          layout="vertical"
          onFinish={handleSubmit}
          initialValues={{ date: selectedDate }}
        >
          <Form.Item
            label={t('appointments.patient')}
          >
            <Space.Compact style={{ display: 'flex', width: '100%' }}>
              <Form.Item
                name="patientId"
                style={{ flex: 1, marginBottom: 0 }}
                rules={[{ required: true, message: 'Patient is required' }]}
              >
                <AutoComplete
                  value={patientSearchText}
                  onSearch={handlePatientSearch}
                  onSelect={handlePatientSelect}
                  placeholder="Search patient by name, phone, or email..."
                  style={{ width: '100%' }}
                  options={patients.map(patient => ({
                    value: patient.id,
                    label: (
                      <div>
                        <div style={{ fontWeight: 'bold' }}>{patient.fullName}</div>
                        <div style={{ fontSize: '12px', color: '#666' }}>
                          {patient.phoneNumber} {patient.email && `• ${patient.email}`}
                        </div>
                        <div style={{ fontSize: '12px', color: '#999' }}>
                          ID: {patient.patientCode}
                        </div>
                      </div>
                    )
                  }))}
                  filterOption={false}
                />
              </Form.Item>
              <Button
                type="primary"
                icon={<PlusOutlined />}
                onClick={() => setIsPatientModalOpen(true)}
                style={{ flexShrink: 0 }}
              >
                New Patient
              </Button>
            </Space.Compact>
          </Form.Item>
          
          {selectedPatient && (
            <div style={{ marginBottom: 16, padding: 12, background: '#f5f5f5', borderRadius: 6 }}>
              <Text strong>Selected Patient:</Text>
              <div><Text strong>{selectedPatient.fullName}</Text></div>
              <div><Text type="secondary">{selectedPatient.phoneNumber}</Text></div>
              {selectedPatient.email && <div><Text type="secondary">{selectedPatient.email}</Text></div>}
              {selectedPatient.allergies && (
                <div><Text type="warning">Allergies: {selectedPatient.allergies}</Text></div>
              )}
              {selectedPatient.chronicConditions && (
                <div><Text type="warning">Conditions: {selectedPatient.chronicConditions}</Text></div>
              )}
            </div>
          )}

          <Form.Item
            name="date"
            label={t('appointments.appointmentDate')}
            rules={[{ required: true, message: 'Date is required' }]}
          >
            <DatePicker showTime style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item
            name="serviceIds"
            label={t('appointments.services')}
            rules={[{ required: true, message: t('appointments.servicesRequired') }]}
          >
            <Select
              mode="multiple"
              placeholder={t('appointments.selectServices')}
              optionFilterProp="children"
            >
              {services.map((service) => (
                <Select.Option key={service.id} value={service.id}>
                  {service.name} - {service.price.toLocaleString()} VND ({service.durationMinutes} {t('services.minutes')})
                </Select.Option>
              ))}
            </Select>
          </Form.Item>

          <Form.Item
            name="status"
            label={t('appointments.status')}
            rules={[{ required: true, message: 'Status is required' }]}
          >
            <Select>
              <Select.Option value="Scheduled">{t('appointments.scheduled')}</Select.Option>
              <Select.Option value="Completed">{t('appointments.completed')}</Select.Option>
              <Select.Option value="Cancelled">{t('appointments.cancelled')}</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item name="notes" label={t('patients.notes')}>
            <Input.TextArea rows={3} />
          </Form.Item>

          <Form.Item>
            <div style={{ display: 'flex', gap: '8px', justifyContent: 'flex-end' }}>
              <Button onClick={() => {
                setIsModalOpen(false);
                resetForm();
              }}>
                {t('common.cancel')}
              </Button>
              <Button type="primary" htmlType="submit">
                {t('common.save')}
              </Button>
            </div>
          </Form.Item>
        </Form>
      </Modal>

      {/* Patient Creation Modal */}
      <Modal
        title={t('patients.addPatient')}
        open={isPatientModalOpen}
        onCancel={handlePatientModalClose}
        footer={null}
        width={800}
      >
        <PatientForm
          patient={null}
          onSuccess={handlePatientCreated}
          onCancel={handlePatientModalClose}
        />
      </Modal>

      {/* Treatment Recording Modal */}
      <Modal
        title={existingTreatment ? 'Update Treatment Record' : 'Record Treatment'}
        open={isTreatmentModalOpen}
        onCancel={handleTreatmentModalClose}
        footer={null}
        width={1000}
        destroyOnClose
      >
        {selectedAppointment && (
          <>
            <div style={{ marginBottom: 16, padding: 12, background: '#f5f5f5', borderRadius: 6 }}>
              <Typography.Text strong>Patient: </Typography.Text>
              <Typography.Text>{selectedAppointment.patientName}</Typography.Text>
              <br />
              <Typography.Text strong>Appointment: </Typography.Text>
              <Typography.Text>{dayjs(selectedAppointment.appointmentDate).format('MMM DD, YYYY HH:mm')}</Typography.Text>
              <br />
              <Typography.Text strong>Services: </Typography.Text>
              <Typography.Text>{selectedAppointment.services?.map(s => s.serviceName).join(', ') || '-'}</Typography.Text>
            </div>
            <TreatmentForm
              treatment={existingTreatment}
              patientId={selectedAppointment.patientId}
              appointmentId={selectedAppointment.id}
              onSuccess={handleTreatmentSuccess}
              onCancel={handleTreatmentModalClose}
            />
          </>
        )}
      </Modal>
    </div>
  );
};

export default Appointments;