import React, { useState, useEffect } from 'react';
import { Table, Button, Form, Input, Modal, message, Space, Tag, Select, DatePicker, Card, Row, Col, Statistic } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, DollarOutlined, ArrowUpOutlined, ArrowDownOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import dayjs from 'dayjs';

const { RangePicker } = DatePicker;

interface Transaction {
  id: number;
  description: string;
  type: 'revenue' | 'expense';
  category: string;
  amount: number;
  date: string;
  paymentMethod: string;
  reference?: string;
}

const Transactions: React.FC = () => {
  const { t } = useTranslation();
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalVisible, setModalVisible] = useState(false);
  const [editingTransaction, setEditingTransaction] = useState<Transaction | null>(null);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchTransactions();
  }, []);

  const fetchTransactions = async () => {
    setLoading(true);
    try {
      // TODO: Replace with actual API call
      const mockData: Transaction[] = [
        {
          id: 1,
          description: 'Patient consultation fee',
          type: 'revenue',
          category: 'Medical Services',
          amount: 500000,
          date: '2024-01-15',
          paymentMethod: 'cash',
          reference: 'INV-001'
        },
        {
          id: 2,
          description: 'Medical supplies purchase',
          type: 'expense',
          category: 'Supplies',
          amount: 200000,
          date: '2024-01-14',
          paymentMethod: 'card',
          reference: 'EXP-001'
        },
        {
          id: 3,
          description: 'Laboratory tests',
          type: 'revenue',
          category: 'Laboratory',
          amount: 300000,
          date: '2024-01-13',
          paymentMethod: 'cash'
        }
      ];
      setTransactions(mockData);
    } catch (error) {
      message.error('Failed to fetch transactions');
    } finally {
      setLoading(false);
    }
  };

  const showModal = (transaction?: Transaction) => {
    setEditingTransaction(transaction || null);
    if (transaction) {
      form.setFieldsValue({
        ...transaction,
        date: dayjs(transaction.date)
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
        date: values.date.format('YYYY-MM-DD'),
        amount: parseFloat(values.amount)
      };

      if (editingTransaction) {
        setTransactions(prev => 
          prev.map(transaction => 
            transaction.id === editingTransaction.id 
              ? { ...editingTransaction, ...formattedValues } 
              : transaction
          )
        );
        message.success('Transaction updated successfully');
      } else {
        const newTransaction: Transaction = {
          id: Date.now(),
          ...formattedValues
        };
        setTransactions(prev => [...prev, newTransaction]);
        message.success('Transaction created successfully');
      }
      setModalVisible(false);
      form.resetFields();
    } catch (error) {
      console.error('Validation failed:', error);
    }
  };

  const handleDelete = (id: number) => {
    Modal.confirm({
      title: 'Are you sure you want to delete this transaction?',
      onOk: () => {
        setTransactions(prev => prev.filter(transaction => transaction.id !== id));
        message.success('Transaction deleted successfully');
      }
    });
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(amount);
  };

  const totalRevenue = transactions
    .filter(t => t.type === 'revenue')
    .reduce((sum, t) => sum + t.amount, 0);

  const totalExpenses = transactions
    .filter(t => t.type === 'expense')
    .reduce((sum, t) => sum + t.amount, 0);

  const netIncome = totalRevenue - totalExpenses;

  const columns = [
    {
      title: 'Description',
      dataIndex: 'description',
      key: 'description',
    },
    {
      title: t('transactions.type'),
      dataIndex: 'type',
      key: 'type',
      render: (type: string) => (
        <Tag color={type === 'revenue' ? 'green' : 'red'} icon={type === 'revenue' ? <ArrowUpOutlined /> : <ArrowDownOutlined />}>
          {type === 'revenue' ? t('transactions.revenue') : t('transactions.expense')}
        </Tag>
      ),
    },
    {
      title: t('transactions.category'),
      dataIndex: 'category',
      key: 'category',
    },
    {
      title: t('transactions.amount'),
      dataIndex: 'amount',
      key: 'amount',
      render: (amount: number, record: Transaction) => (
        <span style={{ color: record.type === 'revenue' ? '#52c41a' : '#ff4d4f' }}>
          {record.type === 'revenue' ? '+' : '-'}{formatCurrency(amount)}
        </span>
      ),
    },
    {
      title: t('transactions.date'),
      dataIndex: 'date',
      key: 'date',
      render: (date: string) => dayjs(date).format('DD/MM/YYYY'),
    },
    {
      title: 'Payment Method',
      dataIndex: 'paymentMethod',
      key: 'paymentMethod',
      render: (method: string) => method.charAt(0).toUpperCase() + method.slice(1),
    },
    {
      title: 'Reference',
      dataIndex: 'reference',
      key: 'reference',
      render: (reference: string) => reference || '-',
    },
    {
      title: t('common.actions'),
      key: 'actions',
      render: (_, record: Transaction) => (
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
      <div style={{ marginBottom: 24 }}>
        <Row gutter={16}>
          <Col span={6}>
            <Card>
              <Statistic
                title="Total Revenue"
                value={totalRevenue}
                formatter={(value) => formatCurrency(Number(value))}
                prefix={<ArrowUpOutlined />}
                valueStyle={{ color: '#3f8600' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Total Expenses"
                value={totalExpenses}
                formatter={(value) => formatCurrency(Number(value))}
                prefix={<ArrowDownOutlined />}
                valueStyle={{ color: '#cf1322' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Net Income"
                value={netIncome}
                formatter={(value) => formatCurrency(Number(value))}
                prefix={<DollarOutlined />}
                valueStyle={{ color: netIncome >= 0 ? '#3f8600' : '#cf1322' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title="Total Transactions"
                value={transactions.length}
                prefix={<DollarOutlined />}
              />
            </Card>
          </Col>
        </Row>
      </div>

      <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h2>{t('menu.transactions')}</h2>
        <Space>
          <RangePicker />
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => showModal()}
          >
            {t('transactions.addTransaction')}
          </Button>
        </Space>
      </div>

      <Table
        columns={columns}
        dataSource={transactions}
        loading={loading}
        rowKey="id"
        pagination={{ pageSize: 10 }}
      />

      <Modal
        title={editingTransaction ? 'Edit Transaction' : t('transactions.addTransaction')}
        open={modalVisible}
        onOk={handleOk}
        onCancel={() => setModalVisible(false)}
        width={600}
      >
        <Form form={form} layout="vertical">
          <Form.Item
            label="Description"
            name="description"
            rules={[{ required: true, message: 'Description is required' }]}
          >
            <Input placeholder="Enter transaction description" />
          </Form.Item>

          <Form.Item
            label={t('transactions.type')}
            name="type"
            rules={[{ required: true, message: 'Type is required' }]}
          >
            <Select placeholder="Select transaction type">
              <Select.Option value="revenue">{t('transactions.revenue')}</Select.Option>
              <Select.Option value="expense">{t('transactions.expense')}</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item
            label={t('transactions.category')}
            name="category"
            rules={[{ required: true, message: 'Category is required' }]}
          >
            <Select placeholder="Select category">
              <Select.Option value="Medical Services">Medical Services</Select.Option>
              <Select.Option value="Laboratory">Laboratory</Select.Option>
              <Select.Option value="Pharmacy">Pharmacy</Select.Option>
              <Select.Option value="Supplies">Supplies</Select.Option>
              <Select.Option value="Equipment">Equipment</Select.Option>
              <Select.Option value="Utilities">Utilities</Select.Option>
              <Select.Option value="Rent">Rent</Select.Option>
              <Select.Option value="Staff Salary">Staff Salary</Select.Option>
              <Select.Option value="Other">Other</Select.Option>
            </Select>
          </Form.Item>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                label={t('transactions.amount')}
                name="amount"
                rules={[{ required: true, message: 'Amount is required' }]}
              >
                <Input type="number" placeholder="Enter amount" />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                label={t('transactions.date')}
                name="date"
                rules={[{ required: true, message: 'Date is required' }]}
                initialValue={dayjs()}
              >
                <DatePicker style={{ width: '100%' }} />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                label="Payment Method"
                name="paymentMethod"
                rules={[{ required: true, message: 'Payment method is required' }]}
              >
                <Select placeholder="Select payment method">
                  <Select.Option value="cash">Cash</Select.Option>
                  <Select.Option value="card">Card</Select.Option>
                  <Select.Option value="bank_transfer">Bank Transfer</Select.Option>
                  <Select.Option value="check">Check</Select.Option>
                </Select>
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                label="Reference"
                name="reference"
              >
                <Input placeholder="Enter reference number" />
              </Form.Item>
            </Col>
          </Row>
        </Form>
      </Modal>
    </div>
  );
};

export default Transactions;