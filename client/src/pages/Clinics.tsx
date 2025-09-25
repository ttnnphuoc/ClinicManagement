import { useState, useEffect } from 'react';
import { Table, Button, Modal, Form, Input, Switch, App, Space, Popconfirm, Dropdown } from 'antd';
import type { MenuProps } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, DownOutlined, CrownOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { clinicService, type Clinic, type CreateClinicRequest } from '../services/clinicService';
import CreateClinicWithPackage from '../components/CreateClinicWithPackage';

const Clinics = () => {
  const { t } = useTranslation();
  const { message } = App.useApp();
  const [clinics, setClinics] = useState<Clinic[]>([]);
  const [loading, setLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isPackageModalOpen, setIsPackageModalOpen] = useState(false);
  const [editingClinic, setEditingClinic] = useState<Clinic | null>(null);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [searchText, setSearchText] = useState('');
  const [form] = Form.useForm();

  const fetchClinics = async () => {
    setLoading(true);
    try {
      const response = await clinicService.getClinics(searchText, page, pageSize);
      if (response.success && response.data) {
        setClinics(response.data.items);
        setTotal(response.data.total);
      }
    } catch (error) {
      message.error(t('errors.UNKNOWN_ERROR'));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchClinics();
  }, [page, searchText]);

  const handleAdd = () => {
    setEditingClinic(null);
    form.resetFields();
    setIsModalOpen(true);
  };

  const handleEdit = (record: Clinic) => {
    setEditingClinic(record);
    form.setFieldsValue(record);
    setIsModalOpen(true);
  };

  const handleDelete = async (id: string) => {
    try {
      const response = await clinicService.deleteClinic(id);
      if (response.success) {
        message.success(t('clinics.deleteSuccess'));
        fetchClinics();
      }
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      message.error(t(`errors.${errorCode}`));
    }
  };

  const handleSubmit = async (values: CreateClinicRequest) => {
    try {
      if (editingClinic) {
        const response = await clinicService.updateClinic(editingClinic.id, values);
        if (response.success) {
          message.success(t('clinics.updateSuccess'));
        }
      } else {
        const response = await clinicService.createClinic(values);
        if (response.success) {
          message.success(t('clinics.createSuccess'));
        }
      }
      setIsModalOpen(false);
      form.resetFields();
      fetchClinics();
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      message.error(t(`errors.${errorCode}`));
    }
  };

  const columns = [
    {
      title: t('clinics.name'),
      dataIndex: 'name',
      key: 'name',
    },
    {
      title: t('clinics.address'),
      dataIndex: 'address',
      key: 'address',
    },
    {
      title: t('clinics.phoneNumber'),
      dataIndex: 'phoneNumber',
      key: 'phoneNumber',
    },
    {
      title: t('clinics.email'),
      dataIndex: 'email',
      key: 'email',
    },
    {
      title: t('clinics.status'),
      dataIndex: 'isActive',
      key: 'isActive',
      render: (isActive: boolean) => (
        <span style={{ color: isActive ? 'green' : 'red' }}>
          {isActive ? t('clinics.active') : t('clinics.inactive')}
        </span>
      ),
    },
    {
      title: t('common.actions'),
      key: 'actions',
      render: (_: any, record: Clinic) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => handleEdit(record)}
          >
            {t('common.edit')}
          </Button>
          <Popconfirm
            title={t('clinics.deleteConfirm')}
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
          placeholder={t('clinics.searchPlaceholder')}
          onSearch={setSearchText}
          style={{ width: 300 }}
          allowClear
        />
        <Dropdown
          menu={{
            items: [
              {
                key: 'regular',
                label: 'Create Regular Clinic',
                icon: <PlusOutlined />,
                onClick: handleAdd,
              },
              {
                key: 'with-package',
                label: 'Create Clinic with Subscription',
                icon: <CrownOutlined />,
                onClick: () => setIsPackageModalOpen(true),
              },
            ] as MenuProps['items'],
          }}
          trigger={['click']}
        >
          <Button type="primary">
            Create Clinic <DownOutlined />
          </Button>
        </Dropdown>
      </div>

      <Table
        columns={columns}
        dataSource={clinics}
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
        title={editingClinic ? t('clinics.editClinic') : t('clinics.addClinic')}
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
            label={t('clinics.name')}
            rules={[{ required: true, message: t('clinics.nameRequired') }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            name="address"
            label={t('clinics.address')}
            rules={[{ required: true, message: t('clinics.addressRequired') }]}
          >
            <Input.TextArea rows={2} />
          </Form.Item>

          <Form.Item
            name="phoneNumber"
            label={t('clinics.phoneNumber')}
            rules={[{ required: true, message: t('clinics.phoneRequired') }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            name="email"
            label={t('clinics.email')}
            rules={[{ type: 'email', message: t('patients.emailInvalid') }]}
          >
            <Input />
          </Form.Item>

          <Form.Item name="isActive" label={t('clinics.status')} valuePropName="checked">
            <Switch checkedChildren={t('clinics.active')} unCheckedChildren={t('clinics.inactive')} />
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

      <CreateClinicWithPackage
        open={isPackageModalOpen}
        onCancel={() => setIsPackageModalOpen(false)}
        onSuccess={() => {
          setIsPackageModalOpen(false);
          fetchClinics();
        }}
      />
    </div>
  );
};

export default Clinics;