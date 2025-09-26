import React, { useState, useEffect } from 'react';
import { Table, Button, Form, Input, Modal, message, Space, Tag, Select, DatePicker, Card, Row, Col, Statistic, Alert } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, WarningOutlined, InboxOutlined, ShoppingCartOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import dayjs from 'dayjs';

interface InventoryItem {
  id: number;
  name: string;
  category: string;
  currentStock: number;
  minimumStock: number;
  unit: string;
  price: number;
  supplier: string;
  expiryDate?: string;
  lastRestocked: string;
  status: 'in_stock' | 'low_stock' | 'out_of_stock';
}

const Inventory: React.FC = () => {
  const { t } = useTranslation();
  const [inventory, setInventory] = useState<InventoryItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalVisible, setModalVisible] = useState(false);
  const [editingItem, setEditingItem] = useState<InventoryItem | null>(null);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchInventory();
  }, []);

  const fetchInventory = async () => {
    setLoading(true);
    try {
      // TODO: Replace with actual API call
      const mockData: InventoryItem[] = [
        {
          id: 1,
          name: 'Paracetamol 500mg',
          category: 'Medicine',
          currentStock: 5,
          minimumStock: 20,
          unit: 'tablets',
          price: 500,
          supplier: 'MedSupply Co.',
          expiryDate: '2024-12-31',
          lastRestocked: '2024-01-10',
          status: 'low_stock'
        },
        {
          id: 2,
          name: 'Surgical Gloves',
          category: 'Medical Supplies',
          currentStock: 100,
          minimumStock: 50,
          unit: 'pairs',
          price: 2000,
          supplier: 'HealthCare Ltd.',
          lastRestocked: '2024-01-15',
          status: 'in_stock'
        },
        {
          id: 3,
          name: 'Insulin Syringes',
          category: 'Medical Equipment',
          currentStock: 0,
          minimumStock: 10,
          unit: 'pieces',
          price: 15000,
          supplier: 'MedDevice Inc.',
          lastRestocked: '2023-12-20',
          status: 'out_of_stock'
        }
      ];
      setInventory(mockData);
    } catch (error) {
      message.error('Failed to fetch inventory');
    } finally {
      setLoading(false);
    }
  };

  const showModal = (item?: InventoryItem) => {
    setEditingItem(item || null);
    if (item) {
      form.setFieldsValue({
        ...item,
        expiryDate: item.expiryDate ? dayjs(item.expiryDate) : null,
        lastRestocked: dayjs(item.lastRestocked)
      });
    } else {
      form.resetFields();
    }
    setModalVisible(true);
  };

  const handleOk = async () => {
    try {
      const values = await form.validateFields();
      const formattedValues = {
        ...values,
        expiryDate: values.expiryDate ? values.expiryDate.format('YYYY-MM-DD') : null,
        lastRestocked: values.lastRestocked.format('YYYY-MM-DD'),
        currentStock: parseInt(values.currentStock),
        minimumStock: parseInt(values.minimumStock),
        price: parseFloat(values.price)
      };

      // Determine status based on stock levels
      let status: 'in_stock' | 'low_stock' | 'out_of_stock';
      if (formattedValues.currentStock === 0) {
        status = 'out_of_stock';
      } else if (formattedValues.currentStock <= formattedValues.minimumStock) {
        status = 'low_stock';
      } else {
        status = 'in_stock';
      }

      if (editingItem) {
        setInventory(prev => 
          prev.map(item => 
            item.id === editingItem.id 
              ? { ...editingItem, ...formattedValues, status } 
              : item
          )
        );
        message.success('Item updated successfully');
      } else {
        const newItem: InventoryItem = {
          id: Date.now(),
          ...formattedValues,
          status
        };
        setInventory(prev => [...prev, newItem]);
        message.success('Item created successfully');
      }
      setModalVisible(false);
      form.resetFields();
    } catch (error) {
      console.error('Validation failed:', error);
    }
  };

  const handleDelete = (id: number) => {
    Modal.confirm({
      title: 'Are you sure you want to delete this item?',
      onOk: () => {
        setInventory(prev => prev.filter(item => item.id !== id));
        message.success('Item deleted successfully');
      }
    });
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(amount);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'in_stock': return 'green';
      case 'low_stock': return 'orange';
      case 'out_of_stock': return 'red';
      default: return 'default';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'in_stock': return 'In Stock';
      case 'low_stock': return 'Low Stock';
      case 'out_of_stock': return 'Out of Stock';
      default: return status;
    }
  };

  const totalItems = inventory.length;
  const lowStockItems = inventory.filter(item => item.status === 'low_stock').length;
  const outOfStockItems = inventory.filter(item => item.status === 'out_of_stock').length;
  const totalValue = inventory.reduce((sum, item) => sum + (item.currentStock * item.price), 0);

  const expiringItems = inventory.filter(item => {
    if (!item.expiryDate) return false;
    const daysUntilExpiry = dayjs(item.expiryDate).diff(dayjs(), 'days');
    return daysUntilExpiry <= 30 && daysUntilExpiry > 0;
  });

  const columns = [
    {
      title: t('inventory.itemName'),
      dataIndex: 'name',
      key: 'name',
    },
    {
      title: 'Category',
      dataIndex: 'category',
      key: 'category',
    },
    {
      title: 'Current Stock',
      dataIndex: 'currentStock',
      key: 'currentStock',
      render: (stock: number, record: InventoryItem) => (
        <span style={{ 
          color: record.status === 'out_of_stock' ? '#ff4d4f' : 
                 record.status === 'low_stock' ? '#faad14' : '#52c41a'
        }}>
          {stock} {record.unit}
        </span>
      ),
    },
    {
      title: 'Min. Stock',
      dataIndex: 'minimumStock',
      key: 'minimumStock',
      render: (stock: number, record: InventoryItem) => `${stock} ${record.unit}`,
    },
    {
      title: 'Price per Unit',
      dataIndex: 'price',
      key: 'price',
      render: (price: number) => formatCurrency(price),
    },
    {
      title: 'Supplier',
      dataIndex: 'supplier',
      key: 'supplier',
    },
    {
      title: 'Expiry Date',
      dataIndex: 'expiryDate',
      key: 'expiryDate',
      render: (date: string) => {
        if (!date) return '-';
        const daysUntilExpiry = dayjs(date).diff(dayjs(), 'days');
        const isExpiringSoon = daysUntilExpiry <= 30 && daysUntilExpiry > 0;
        return (
          <span style={{ color: isExpiringSoon ? '#faad14' : 'inherit' }}>
            {dayjs(date).format('DD/MM/YYYY')}
            {isExpiringSoon && <WarningOutlined style={{ marginLeft: 4, color: '#faad14' }} />}
          </span>
        );
      },
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      render: (status: string) => (
        <Tag color={getStatusColor(status)}>
          {getStatusText(status)}
        </Tag>
      ),
    },
    {
      title: t('common.actions'),
      key: 'actions',
      render: (_, record: InventoryItem) => (
        <Space>
          <Button
            type="primary"
            icon={<EditOutlined />}
            size="small"
            onClick={() => showModal(record)}
          >
            {t('common.edit')}
          </Button>
          <Button
            danger
            icon={<DeleteOutlined />}
            size="small"
            onClick={() => handleDelete(record.id)}
          >
            {t('common.delete')}
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div>
      {(lowStockItems > 0 || outOfStockItems > 0 || expiringItems.length > 0) && (
        <div style={{ marginBottom: 16 }}>
          {outOfStockItems > 0 && (
            <Alert
              message={`${outOfStockItems} item(s) are out of stock`}
              type="error"
              showIcon
              style={{ marginBottom: 8 }}
            />
          )}
          {lowStockItems > 0 && (
            <Alert
              message={`${lowStockItems} item(s) are running low on stock`}
              type="warning"
              showIcon
              style={{ marginBottom: 8 }}
            />
          )}
          {expiringItems.length > 0 && (
            <Alert
              message={`${expiringItems.length} item(s) will expire within 30 days`}
              type="warning"
              showIcon
              style={{ marginBottom: 8 }}
            />
          )}
        </div>
      )}

      <div style={{ marginBottom: 24 }}>
        <Row gutter={16}>
          <Col span={6}>
            <Card>
              <Statistic
                title="Total Items"
                value={totalItems}
                prefix={<InboxOutlined />}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Low Stock Items"
                value={lowStockItems}
                prefix={<WarningOutlined />}
                valueStyle={{ color: '#faad14' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Out of Stock"
                value={outOfStockItems}
                prefix={<WarningOutlined />}
                valueStyle={{ color: '#ff4d4f' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Total Value"
                value={totalValue}
                formatter={(value) => formatCurrency(Number(value))}
                prefix={<ShoppingCartOutlined />}
              />
            </Card>
          </Col>
        </Row>
      </div>

      <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h2>{t('menu.inventory')}</h2>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => showModal()}
        >
          {t('inventory.addItem')}
        </Button>
      </div>

      <Table
        columns={columns}
        dataSource={inventory}
        loading={loading}
        rowKey="id"
        pagination={{ pageSize: 10 }}
      />

      <Modal
        title={editingItem ? 'Edit Item' : t('inventory.addItem')}
        open={modalVisible}
        onOk={handleOk}
        onCancel={() => setModalVisible(false)}
        width={800}
      >
        <Form form={form} layout="vertical">
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                label={t('inventory.itemName')}
                name="name"
                rules={[{ required: true, message: 'Item name is required' }]}
              >
                <Input placeholder="Enter item name" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                label="Category"
                name="category"
                rules={[{ required: true, message: 'Category is required' }]}
              >
                <Select placeholder="Select category">
                  <Select.Option value="Medicine">Medicine</Select.Option>
                  <Select.Option value="Medical Supplies">Medical Supplies</Select.Option>
                  <Select.Option value="Medical Equipment">Medical Equipment</Select.Option>
                  <Select.Option value="Office Supplies">Office Supplies</Select.Option>
                  <Select.Option value="Cleaning Supplies">Cleaning Supplies</Select.Option>
                </Select>
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={8}>
              <Form.Item
                label="Current Stock"
                name="currentStock"
                rules={[{ required: true, message: 'Current stock is required' }]}
              >
                <Input type="number" placeholder="Enter current stock" />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item
                label="Minimum Stock"
                name="minimumStock"
                rules={[{ required: true, message: 'Minimum stock is required' }]}
              >
                <Input type="number" placeholder="Enter minimum stock" />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item
                label={t('inventory.unit')}
                name="unit"
                rules={[{ required: true, message: 'Unit is required' }]}
              >
                <Select placeholder="Select unit">
                  <Select.Option value="pieces">Pieces</Select.Option>
                  <Select.Option value="tablets">Tablets</Select.Option>
                  <Select.Option value="bottles">Bottles</Select.Option>
                  <Select.Option value="boxes">Boxes</Select.Option>
                  <Select.Option value="pairs">Pairs</Select.Option>
                  <Select.Option value="kg">Kg</Select.Option>
                  <Select.Option value="liters">Liters</Select.Option>
                </Select>
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                label="Price per Unit"
                name="price"
                rules={[{ required: true, message: 'Price is required' }]}
              >
                <Input type="number" placeholder="Enter price per unit" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                label="Supplier"
                name="supplier"
                rules={[{ required: true, message: 'Supplier is required' }]}
              >
                <Input placeholder="Enter supplier name" />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                label={t('inventory.expiryDate')}
                name="expiryDate"
              >
                <DatePicker style={{ width: '100%' }} placeholder="Select expiry date" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                label="Last Restocked"
                name="lastRestocked"
                rules={[{ required: true, message: 'Last restocked date is required' }]}
                initialValue={dayjs()}
              >
                <DatePicker style={{ width: '100%' }} />
              </Form.Item>
            </Col>
          </Row>
        </Form>
      </Modal>
    </div>
  );
};

export default Inventory;