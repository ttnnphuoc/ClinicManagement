import { useState, useEffect } from 'react';
import { Table, Button, Modal, Form, Input, Select, Switch, App, Space, Popconfirm, Tag } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, KeyOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { staffService, type Staff, type CreateStaffRequest, type UpdateStaffRequest } from '../services/staffService';
import { clinicService, type Clinic } from '../services/clinicService';

const { Option } = Select;

const Staff = () => {
  const { t } = useTranslation();
  const { message } = App.useApp();
  const [staff, setStaff] = useState<Staff[]>([]);
  const [clinics, setClinics] = useState<Clinic[]>([]);
  const [loading, setLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isPasswordModalOpen, setIsPasswordModalOpen] = useState(false);
  const [editingStaff, setEditingStaff] = useState<Staff | null>(null);
  const [selectedStaffId, setSelectedStaffId] = useState<string | null>(null);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [searchText, setSearchText] = useState('');
  const [form] = Form.useForm();
  const [passwordForm] = Form.useForm();

  const fetchStaff = async () => {
    setLoading(true);
    try {
      const response = await staffService.getStaff(searchText, page, pageSize);
      if (response.success && response.data) {
        setStaff(response.data.items);
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
      const response = await clinicService.getActiveClinics();
      if (response.success && response.data) {
        setClinics(response.data);
      }
    } catch (error) {
      console.error('Failed to fetch clinics:', error);
    }
  };

  useEffect(() => {
    fetchStaff();
  }, [page, searchText]);

  useEffect(() => {
    fetchClinics();
  }, []);

  const handleAdd = () => {
    setEditingStaff(null);
    form.resetFields();
    setIsModalOpen(true);
  };

  const handleEdit = (record: Staff) => {
    setEditingStaff(record);
    form.setFieldsValue({
      ...record,
      clinicIds: record.clinics.map(c => c.clinicId),
    });
    setIsModalOpen(true);
  };

  const handleChangePassword = (record: Staff) => {
    setSelectedStaffId(record.id);
    passwordForm.resetFields();
    setIsPasswordModalOpen(true);
  };

  const handleDelete = async (id: string) => {
    try {
      const response = await staffService.deleteStaff(id);
      if (response.success) {
        message.success(t('staff.deleteSuccess'));
        fetchStaff();
      }
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      message.error(t(`errors.${errorCode}`));
    }
  };

  const handleSubmit = async (values: CreateStaffRequest | UpdateStaffRequest) => {
    try {
      if (editingStaff) {
        const response = await staffService.updateStaff(editingStaff.id, values as UpdateStaffRequest);
        if (response.success) {
          message.success(t('staff.updateSuccess'));
        }
      } else {
        const response = await staffService.createStaff(values as CreateStaffRequest);
        if (response.success) {
          message.success(t('staff.createSuccess'));
        }
      }
      setIsModalOpen(false);
      form.resetFields();
      fetchStaff();
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      message.error(t(`errors.${errorCode}`));
    }
  };

  const handlePasswordSubmit = async (values: { newPassword: string }) => {
    if (!selectedStaffId) return;
    
    try {
      const response = await staffService.changePassword(selectedStaffId, values.newPassword);
      if (response.success) {
        message.success(t('staff.passwordChanged'));
      }
      setIsPasswordModalOpen(false);
      passwordForm.resetFields();
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      message.error(t(`errors.${errorCode}`));
    }
  };

  const columns = [
    {
      title: t('staff.fullName'),
      dataIndex: 'fullName',
      key: 'fullName',
    },
    {
      title: t('staff.email'),
      dataIndex: 'email',
      key: 'email',
    },
    {
      title: t('staff.phoneNumber'),
      dataIndex: 'phoneNumber',
      key: 'phoneNumber',
    },
    {
      title: t('staff.role'),
      dataIndex: 'role',
      key: 'role',
      render: (role: string) => t(`staff.roles.${role}`),
    },
    {
      title: t('staff.clinics'),
      key: 'clinics',
      render: (record: Staff) => (
        <Space wrap>
          {record.clinics.map(clinic => (
            <Tag key={clinic.clinicId} color={clinic.isActive ? 'green' : 'default'}>
              {clinic.clinicName}
            </Tag>
          ))}
        </Space>
      ),
    },
    {
      title: t('staff.status'),
      dataIndex: 'isActive',
      key: 'isActive',
      render: (isActive: boolean) => (
        <Tag color={isActive ? 'green' : 'red'}>
          {isActive ? t('staff.active') : t('staff.inactive')}
        </Tag>
      ),
    },
    {
      title: t('common.actions'),
      key: 'actions',
      render: (record: Staff) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => handleEdit(record)}
          />
          <Button
            type="link"
            icon={<KeyOutlined />}
            onClick={() => handleChangePassword(record)}
          />
          <Popconfirm
            title={t('staff.deleteConfirm')}
            onConfirm={() => handleDelete(record.id)}
            okText={t('common.yes')}
            cancelText={t('common.no')}
          >
            <Button type="link" danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between' }}>
        <Input.Search
          placeholder={t('staff.searchPlaceholder')}
          allowClear
          style={{ width: 300 }}
          onSearch={setSearchText}
        />
        <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
          {t('staff.addStaff')}
        </Button>
      </div>

      <Table
        columns={columns}
        dataSource={staff}
        rowKey="id"
        loading={loading}
        pagination={{
          current: page,
          pageSize,
          total,
          onChange: setPage,
        }}
      />

      <Modal
        title={editingStaff ? t('staff.editStaff') : t('staff.addStaff')}
        open={isModalOpen}
        onCancel={() => setIsModalOpen(false)}
        onOk={() => form.submit()}
      >
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
          <Form.Item
            name="fullName"
            label={t('staff.fullName')}
            rules={[{ required: true, message: t('staff.fullNameRequired') }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            name="email"
            label={t('staff.email')}
            rules={[
              { required: true, message: t('staff.emailRequired') },
              { type: 'email', message: t('staff.emailInvalid') },
            ]}
          >
            <Input />
          </Form.Item>

          {!editingStaff && (
            <Form.Item
              name="password"
              label={t('staff.password')}
              rules={[{ required: true, message: t('staff.passwordRequired') }]}
            >
              <Input.Password />
            </Form.Item>
          )}

          <Form.Item
            name="phoneNumber"
            label={t('staff.phoneNumber')}
            rules={[{ required: true, message: t('staff.phoneRequired') }]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            name="role"
            label={t('staff.role')}
            rules={[{ required: true, message: t('staff.roleRequired') }]}
          >
            <Select>
              <Option value="SuperAdmin">{t('staff.roles.SuperAdmin')}</Option>
              <Option value="ClinicManager">{t('staff.roles.ClinicManager')}</Option>
              <Option value="Doctor">{t('staff.roles.Doctor')}</Option>
              <Option value="Nurse">{t('staff.roles.Nurse')}</Option>
              <Option value="Receptionist">{t('staff.roles.Receptionist')}</Option>
              <Option value="Accountant">{t('staff.roles.Accountant')}</Option>
              <Option value="Pharmacist">{t('staff.roles.Pharmacist')}</Option>
            </Select>
          </Form.Item>

          <Form.Item
            name="clinicIds"
            label={t('staff.clinics')}
            rules={[{ required: true, message: t('staff.clinicsRequired') }]}
          >
            <Select mode="multiple" placeholder={t('staff.selectClinics')}>
              {clinics.map(clinic => (
                <Option key={clinic.id} value={clinic.id}>
                  {clinic.name}
                </Option>
              ))}
            </Select>
          </Form.Item>

          <Form.Item name="isActive" label={t('staff.status')} valuePropName="checked" initialValue={true}>
            <Switch checkedChildren={t('staff.active')} unCheckedChildren={t('staff.inactive')} />
          </Form.Item>
        </Form>
      </Modal>

      <Modal
        title={t('staff.changePassword')}
        open={isPasswordModalOpen}
        onCancel={() => setIsPasswordModalOpen(false)}
        onOk={() => passwordForm.submit()}
      >
        <Form form={passwordForm} layout="vertical" onFinish={handlePasswordSubmit}>
          <Form.Item
            name="newPassword"
            label={t('staff.newPassword')}
            rules={[
              { required: true, message: t('staff.passwordRequired') },
              { min: 6, message: t('staff.passwordMinLength') },
            ]}
          >
            <Input.Password />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default Staff;