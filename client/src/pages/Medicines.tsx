import { useState, useEffect } from 'react';
import { Table, Button, Modal, Form, Input, InputNumber, Switch, App, Space, Popconfirm, Select, Tag, Card, Statistic } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, SearchOutlined, MedicineBoxOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { medicineService, type Medicine, type CreateMedicineRequest } from '../services/medicineService';

const { Option } = Select;

const MEDICINE_FORMS = [
  'Tablet',
  'Capsule', 
  'Syrup',
  'Injection',
  'Cream',
  'Ointment',
  'Drops',
  'Powder',
  'Solution'
];

const Medicines = () => {
  const { t } = useTranslation();
  const { message } = App.useApp();
  const [medicines, setMedicines] = useState<Medicine[]>([]);
  const [loading, setLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingMedicine, setEditingMedicine] = useState<Medicine | null>(null);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchMedicines();
  }, []);

  const fetchMedicines = async () => {
    setLoading(true);
    try {
      const response = await medicineService.getMedicines();
      if (response.success && response.data) {
        setMedicines(response.data);
      }
    } catch (error) {
      message.error(t('errors.UNKNOWN_ERROR'));
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async (value: string) => {
    if (!value.trim()) {
      fetchMedicines();
      return;
    }

    setLoading(true);
    try {
      const response = await medicineService.searchMedicines(value);
      if (response.success && response.data) {
        setMedicines(response.data);
      }
    } catch (error) {
      message.error(t('errors.UNKNOWN_ERROR'));
    } finally {
      setLoading(false);
    }
  };

  const handleAdd = () => {
    setEditingMedicine(null);
    form.resetFields();
    setIsModalOpen(true);
  };

  const handleEdit = (record: Medicine) => {
    setEditingMedicine(record);
    form.setFieldsValue(record);
    setIsModalOpen(true);
  };

  const handleDelete = async (id: string) => {
    try {
      const response = await medicineService.deleteMedicine(id);
      if (response.success) {
        message.success(t('medicines.deleteSuccess'));
        fetchMedicines();
      }
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      message.error(t(`errors.${errorCode}`));
    }
  };

  const handleSubmit = async (values: CreateMedicineRequest) => {
    try {
      if (editingMedicine) {
        const response = await medicineService.updateMedicine(editingMedicine.id, {
          ...values,
          isActive: values.isActive ?? true
        });
        if (response.success) {
          message.success(t('medicines.updateSuccess'));
        }
      } else {
        const response = await medicineService.createMedicine(values);
        if (response.success) {
          message.success(t('medicines.createSuccess'));
        }
      }
      setIsModalOpen(false);
      form.resetFields();
      fetchMedicines();
    } catch (error: any) {
      const errorCode = error.response?.data?.code || 'UNKNOWN_ERROR';
      message.error(t(`errors.${errorCode}`));
    }
  };

  const getStockStatusColor = (stock: number) => {
    if (stock === 0) return 'red';
    if (stock < 10) return 'orange';
    if (stock < 50) return 'gold';
    return 'green';
  };

  const getStockStatusText = (stock: number) => {
    if (stock === 0) return 'Out of Stock';
    if (stock < 10) return 'Low Stock';
    if (stock < 50) return 'Medium Stock';
    return 'In Stock';
  };

  const columns = [
    {
      title: t('medicines.name'),
      dataIndex: 'name',
      key: 'name',
      sorter: (a: Medicine, b: Medicine) => a.name.localeCompare(b.name),
    },
    {
      title: t('medicines.genericName'),
      dataIndex: 'genericName',
      key: 'genericName',
      render: (text: string) => text || '-',
    },
    {
      title: t('medicines.manufacturer'),
      dataIndex: 'manufacturer',
      key: 'manufacturer',
      render: (text: string) => text || '-',
    },
    {
      title: t('medicines.dosage'),
      dataIndex: 'dosage',
      key: 'dosage',
      render: (text: string) => text || '-',
    },
    {
      title: t('medicines.form'),
      dataIndex: 'form',
      key: 'form',
      render: (text: string) => text || '-',
    },
    {
      title: t('medicines.price'),
      dataIndex: 'price',
      key: 'price',
      render: (price: number) => `${Number(price).toLocaleString()} VND`,
      sorter: (a: Medicine, b: Medicine) => a.price - b.price,
    },
    {
      title: t('medicines.stock'),
      dataIndex: 'totalStock',
      key: 'totalStock',
      render: (stock: number) => (
        <Tag color={getStockStatusColor(stock)}>
          {stock} - {getStockStatusText(stock)}
        </Tag>
      ),
      sorter: (a: Medicine, b: Medicine) => a.totalStock - b.totalStock,
    },
    {
      title: t('medicines.status'),
      dataIndex: 'isActive',
      key: 'isActive',
      render: (isActive: boolean) => (
        <span style={{ color: isActive ? 'green' : 'red' }}>
          {isActive ? t('medicines.active') : t('medicines.inactive')}
        </span>
      ),
      filters: [
        { text: t('medicines.active'), value: true },
        { text: t('medicines.inactive'), value: false },
      ],
      onFilter: (value: any, record: Medicine) => record.isActive === value,
    },
    {
      title: t('common.actions'),
      key: 'actions',
      render: (_: any, record: Medicine) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => handleEdit(record)}
          >
            {t('common.edit')}
          </Button>
          <Popconfirm
            title={t('medicines.deleteConfirm')}
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

  const totalMedicines = medicines.length;
  const activeMedicines = medicines.filter(m => m.isActive).length;
  const lowStockMedicines = medicines.filter(m => m.totalStock < 10).length;
  const outOfStockMedicines = medicines.filter(m => m.totalStock === 0).length;

  return (
    <div>
      {/* Statistics Cards */}
      <div style={{ marginBottom: 24 }}>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: 16 }}>
          <Card>
            <Statistic
              title={t('medicines.totalMedicines')}
              value={totalMedicines}
              prefix={<MedicineBoxOutlined />}
            />
          </Card>
          <Card>
            <Statistic
              title={t('medicines.activeMedicines')}
              value={activeMedicines}
              valueStyle={{ color: '#3f8600' }}
            />
          </Card>
          <Card>
            <Statistic
              title={t('medicines.lowStock')}
              value={lowStockMedicines}
              valueStyle={{ color: '#cf1322' }}
            />
          </Card>
          <Card>
            <Statistic
              title={t('medicines.outOfStock')}
              value={outOfStockMedicines}
              valueStyle={{ color: '#ff4d4f' }}
            />
          </Card>
        </div>
      </div>

      {/* Search and Add Section */}
      <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between' }}>
        <Input.Search
          placeholder={t('medicines.searchPlaceholder')}
          allowClear
          enterButton={<SearchOutlined />}
          size="large"
          style={{ width: 400 }}
          onSearch={handleSearch}
          onChange={(e) => {
            const value = e.target.value;
            if (!value) {
              fetchMedicines();
            }
          }}
        />
        <Button type="primary" size="large" icon={<PlusOutlined />} onClick={handleAdd}>
          {t('medicines.addMedicine')}
        </Button>
      </div>

      {/* Medicines Table */}
      <Table
        columns={columns}
        dataSource={medicines}
        rowKey="id"
        loading={loading}
        pagination={{
          pageSize: 10,
          showTotal: (total) => t('common.totalItems', { total }),
          showSizeChanger: true,
          showQuickJumper: true,
        }}
      />

      {/* Add/Edit Medicine Modal */}
      <Modal
        title={editingMedicine ? t('medicines.editMedicine') : t('medicines.addMedicine')}
        open={isModalOpen}
        onCancel={() => {
          setIsModalOpen(false);
          form.resetFields();
        }}
        footer={null}
        width={800}
      >
        <Form
          form={form}
          layout="vertical"
          onFinish={handleSubmit}
          initialValues={{ isActive: true }}
        >
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16 }}>
            <Form.Item
              name="name"
              label={t('medicines.name')}
              rules={[{ required: true, message: t('medicines.nameRequired') }]}
            >
              <Input placeholder={t('medicines.enterMedicineName')} />
            </Form.Item>

            <Form.Item
              name="genericName"
              label={t('medicines.genericName')}
            >
              <Input placeholder={t('medicines.enterGenericName')} />
            </Form.Item>

            <Form.Item
              name="manufacturer"
              label={t('medicines.manufacturer')}
            >
              <Input placeholder={t('medicines.enterManufacturer')} />
            </Form.Item>

            <Form.Item
              name="dosage"
              label={t('medicines.dosage')}
            >
              <Input placeholder={t('medicines.enterDosage')} />
            </Form.Item>

            <Form.Item
              name="form"
              label={t('medicines.form')}
            >
              <Select placeholder={t('medicines.selectForm')} allowClear>
                {MEDICINE_FORMS.map(form => (
                  <Option key={form} value={form}>{form}</Option>
                ))}
              </Select>
            </Form.Item>

            <Form.Item
              name="price"
              label={t('medicines.price')}
              rules={[{ required: true, message: t('medicines.priceRequired') }]}
            >
              <InputNumber
                style={{ width: '100%' }}
                min={0}
                step={0.01}
                controls={false}
                formatter={(value) => `${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
                addonAfter="VND"
                placeholder={t('medicines.enterPrice')}
              />
            </Form.Item>
          </div>

          <Form.Item
            name="description"
            label={t('medicines.description')}
          >
            <Input.TextArea rows={3} placeholder={t('medicines.enterDescription')} />
          </Form.Item>

          {editingMedicine && (
            <Form.Item name="isActive" label={t('medicines.status')} valuePropName="checked">
              <Switch checkedChildren={t('medicines.active')} unCheckedChildren={t('medicines.inactive')} />
            </Form.Item>
          )}

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

export default Medicines;