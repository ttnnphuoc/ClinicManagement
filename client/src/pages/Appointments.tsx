import { Calendar, Badge, Modal, Form, Input, DatePicker, Select, Button, App } from 'antd';
import { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import type { Dayjs } from 'dayjs';
import dayjs from 'dayjs';
import { serviceService, type Service } from '../services/serviceService';

interface Appointment {
  id: string;
  title: string;
  date: string;
  type: 'warning' | 'success' | 'error' | 'default';
  patientName: string;
  status: string;
}

const Appointments = () => {
  const { t } = useTranslation();
  const { message } = App.useApp();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(null);
  const [services, setServices] = useState<Service[]>([]);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchServices();
  }, []);

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

  const [appointments] = useState<Appointment[]>([
    {
      id: '1',
      title: 'Checkup - John Doe',
      date: dayjs().format('YYYY-MM-DD'),
      type: 'success',
      patientName: 'John Doe',
      status: 'Scheduled'
    },
    {
      id: '2',
      title: 'Emergency - Jane Smith',
      date: dayjs().add(2, 'day').format('YYYY-MM-DD'),
      type: 'error',
      patientName: 'Jane Smith',
      status: 'Urgent'
    }
  ]);

  const getListData = (value: Dayjs) => {
    const dateStr = value.format('YYYY-MM-DD');
    return appointments.filter(apt => apt.date === dateStr);
  };

  const dateCellRender = (value: Dayjs) => {
    const listData = getListData(value);
    return (
      <ul style={{ listStyle: 'none', padding: 0 }}>
        {listData.map((item) => (
          <li key={item.id}>
            <Badge status={item.type} text={item.title} />
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

  const handleSubmit = async (values: any) => {
    try {
      console.log('Appointment:', {
        ...values,
        date: values.date.format('YYYY-MM-DD HH:mm'),
      });
      message.success(t('appointments.createSuccess'));
      setIsModalOpen(false);
      form.resetFields();
    } catch (error) {
      message.error(t('errors.UNKNOWN_ERROR'));
    }
  };

  return (
    <div>
      <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h2 style={{ margin: 0 }}>{t('appointments.title')}</h2>
        <Button type="primary" onClick={() => setIsModalOpen(true)}>
          {t('appointments.addAppointment')}
        </Button>
      </div>

      <Calendar 
        dateCellRender={dateCellRender}
        onSelect={onSelect}
      />

      <Modal
        title={t('appointments.addAppointment')}
        open={isModalOpen}
        onCancel={() => {
          setIsModalOpen(false);
          form.resetFields();
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
            name="patientName"
            label={t('appointments.patient')}
            rules={[{ required: true, message: 'Patient name is required' }]}
          >
            <Input />
          </Form.Item>

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
                form.resetFields();
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
    </div>
  );
};

export default Appointments;