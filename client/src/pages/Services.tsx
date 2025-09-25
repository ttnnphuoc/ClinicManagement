import { useState, useEffect } from 'react';
import { Table, Button, Modal, Form, Input, InputNumber, Switch, App, Space, Popconfirm, Select } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { serviceService, type Service, type CreateServiceRequest } from '../services/serviceService';
import { clinicService, type Clinic } from '../services/clinicService';

const { Option } = Select;

const Services = () => {
  const { t } = useTranslation();
  const { message } = App.useApp();
  const [services, setServices] = useState<Service[]>([]);
  const [clinics, setClinics] = useState<Clinic[]>([]);
  const [loading, setLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingService, setEditingService] = useState<Service | null>(null);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [searchText, setSearchText] = useState('');
  const [form] = Form.useForm();

  const user = JSON.parse(localStorage.getItem('user') || '{}');
  const currentClinic = JSON.parse(localStorage.getItem('currentClinic') || 'null');
  const isSuperAdmin = user.role === 'SuperAdmin';
  const isClinicManager = user.role === 'ClinicManager';

  const fetchServices = async () => {
    setLoading(true);
    try {
      const response = await serviceService.getServices(searchText, page, pageSize);
      if (response.success && response.data) {
        setServices(response.data.items);
        setTotal(response.data.total);
      }
    } catch (error) {
      message.error(t('errors.UNKNOWN_ERROR'));
    } finally {
      setLoading(false);
    }
  };

  const fetchClinics = async () => {
    try {
      let response;
      if (isSuperAdmin) {
        response = await clinicService.getActiveClinics();
      } else {
        response = await clinicService.getMyClinics();
      }
      
      if (response.success && response.data) {
        setClinics(response.data);
      }
    } catch (error) {
      console.error('Failed to fetch clinics:', error);
    }
  };

  useEffect(() => {
    fetchServices();
  }, [page, searchText]);

  useEffect(() => {
    if (isSuperAdmin || isClinicManager) {
      fetchClinics();
    }
  }, []);

  const handleAdd = () => {
    setEditingService(null);
    form.resetFields();
    if (isClinicManager) {
      if (clinics.length === 1) {
        form.setFieldsValue({ clinicId: clinics[0].id });
      } else if (currentClinic) {
        form.setFieldsValue({ clinicId: currentClinic.id });
      }
    }
    setIsModalOpen(true);
  };

  const handleEdit = (record: Service) => {
    setEditingService(record);
    form.setFieldsValue(record);
    setIsModalOpen(true);
  };

  const handleDelete = async (id: string) => {
    try {
      const response = await serviceService.deleteService(id);
      if (response.success) {
        message.success(t('services.deleteSuccess'));
        fetchServices();
      }
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      message.error(t(`errors.${errorCode}`));
    }
  };

  const handleSubmit = async (values: CreateServiceRequest) => {
    try {
      if (editingService) {
        const response = await serviceService.updateService(editingService.id, values);
        if (response.success) {
          message.success(t('services.updateSuccess'));
        }
      } else {
        const response = await serviceService.createService(values);
        if (response.success) {
          message.success(t('services.createSuccess'));
        }
      }
      setIsModalOpen(false);
      form.resetFields();
      fetchServices();
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      message.error(t(`errors.${errorCode}`));
    }
  };

  const columns = [
    {
      title: t('services.serviceName'),
      dataIndex: 'name',
      key: 'name',
    },
    {
      title: t('services.description'),
      dataIndex: 'description',
      key: 'description',
    },
    {
      title: t('services.price'),
      dataIndex: 'price',
      key: 'price',
      render: (price: number) => `${Number(price).toLocaleString()} VND`,
    },
    {
      title: t('services.duration'),
      dataIndex: 'durationMinutes',
      key: 'durationMinutes',
      render: (minutes: number) => `${minutes} ${t('services.minutes')}`,
    },
    {
      title: t('services.status'),
      dataIndex: 'isActive',
      key: 'isActive',
      render: (isActive: boolean) => (
        <span style={{ color: isActive ? 'green' : 'red' }}>
          {isActive ? t('services.active') : t('services.inactive')}
        </span>
      ),
    },
    {
      title: t('common.actions'),
      key: 'actions',
      render: (_: any, record: Service) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => handleEdit(record)}
          >
            {t('common.edit')}
          </Button>
          <Popconfirm
            title={t('services.deleteConfirm')}
            onConfirm={() => handleDelete(record.id)}
            okText={t('common.yes')}
            cancelText={t('common.no')}
          >
            <Button type="link" danger icon={<DeleteOutlined />}>
              {t('common.delete')}
            </Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between' }}>
        <Input.Search
          placeholder={t('services.searchPlaceholder')}
          onSearch={setSearchText}
          style={{ width: 300 }}
          allowClear
        />
        <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
          {t('services.addService')}
        </Button>
      </div>

      <Table
        columns={columns}
        dataSource={services}
        rowKey="id"
        loading={loading}
        pagination={{
          current: page,
          pageSize,
          total,
          onChange: setPage,
          showTotal: (total) => t('common.totalItems', { total }),
        }}
      />

      <Modal
        title={editingService ? t('services.editService') : t('services.addService')}
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
          initialValues={{ isActive: true }}
        >
          <Form.Item
            name="name"
            label={t('services.serviceName')}
            rules={[{ required: true, message: t('services.nameRequired') }]}
          >
            <Input />
          </Form.Item>

          <Form.Item name="description" label={t('services.description')}>
            <Input.TextArea rows={3} />
          </Form.Item>

          <Form.Item
            name="price"
            label={t('services.price')}
            rules={[{ required: true, message: t('services.priceRequired') }]}
          >
            <InputNumber
              style={{ width: '100%' }}
              min={0}
              controls={false}
              formatter={(value) => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
              parser={(value) => value?.replace(/[^\d]/g, '') as any}
              addonAfter="VND"
            />
          </Form.Item>

          <Form.Item
            name="durationMinutes"
            label={t('services.duration')}
            rules={[{ required: true, message: t('services.durationRequired') }]}
          >
            <InputNumber
              style={{ width: '100%' }}
              min={1}
              addonAfter={t('services.minutes')}
            />
          </Form.Item>

          {isSuperAdmin && (
            <Form.Item
              name="clinicId"
              label={t('services.clinic')}
              rules={[{ required: true, message: t('services.clinicRequired') }]}
            >
              <Select placeholder={t('services.selectClinic')}>
                {clinics.map(clinic => (
                  <Option key={clinic.id} value={clinic.id}>
                    {clinic.name}
                  </Option>
                ))}
              </Select>
            </Form.Item>
          )}

          {isClinicManager && (
            <Form.Item
              name="clinicId"
              label={t('services.clinic')}
              rules={[{ required: true, message: t('services.clinicRequired') }]}
            >
              {clinics.length > 1 ? (
                <Select placeholder={t('services.selectClinic')}>
                  {clinics.map(clinic => (
                    <Option key={clinic.id} value={clinic.id}>
                      {clinic.name}
                    </Option>
                  ))}
                </Select>
              ) : (
                <Input value={currentClinic?.name || clinics[0]?.name} disabled />
              )}
            </Form.Item>
          )}

          <Form.Item name="isActive" label={t('services.status')} valuePropName="checked">
            <Switch checkedChildren={t('services.active')} unCheckedChildren={t('services.inactive')} />
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

export default Services;