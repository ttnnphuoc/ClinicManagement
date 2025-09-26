import React, { useState, useEffect } from 'react';
import { Table, Button, Space, Tag, DatePicker, Input, Modal, Descriptions, Card } from 'antd';
import { EyeOutlined, DownloadOutlined, SearchOutlined, PrinterOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import dayjs from 'dayjs';

const { RangePicker } = DatePicker;

interface Receipt {
  id: number;
  receiptNumber: string;
  patientName: string;
  date: string;
  amount: number;
  paymentMethod: 'cash' | 'card' | 'insurance';
  status: 'paid' | 'pending' | 'refunded';
  services: Array<{
    name: string;
    price: number;
  }>;
}

const Receipts: React.FC = () => {
  const { t } = useTranslation();
  const [receipts, setReceipts] = useState<Receipt[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchText, setSearchText] = useState('');
  const [selectedReceipt, setSelectedReceipt] = useState<Receipt | null>(null);
  const [detailModalVisible, setDetailModalVisible] = useState(false);

  useEffect(() => {
    fetchReceipts();
  }, []);

  const fetchReceipts = async () => {
    setLoading(true);
    try {
      // TODO: Replace with actual API call
      const mockData: Receipt[] = [
        {
          id: 1,
          receiptNumber: 'RC-2024-001',
          patientName: 'Nguyen Van A',
          date: '2024-01-15',
          amount: 500000,
          paymentMethod: 'cash',
          status: 'paid',
          services: [
            { name: 'General Consultation', price: 200000 },
            { name: 'Blood Test', price: 300000 }
          ]
        },
        {
          id: 2,
          receiptNumber: 'RC-2024-002',
          patientName: 'Tran Thi B',
          date: '2024-01-16',
          amount: 750000,
          paymentMethod: 'card',
          status: 'paid',
          services: [
            { name: 'X-Ray', price: 750000 }
          ]
        }
      ];
      setReceipts(mockData);
    } catch (error) {
      console.error('Failed to fetch receipts:', error);
    } finally {
      setLoading(false);
    }
  };

  const showReceiptDetail = (receipt: Receipt) => {
    setSelectedReceipt(receipt);
    setDetailModalVisible(true);
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(amount);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'paid': return 'green';
      case 'pending': return 'orange';
      case 'refunded': return 'red';
      default: return 'default';
    }
  };

  const getPaymentMethodText = (method: string) => {
    switch (method) {
      case 'cash': return 'Cash';
      case 'card': return 'Card';
      case 'insurance': return 'Insurance';
      default: return method;
    }
  };

  const columns = [
    {
      title: 'Receipt No.',
      dataIndex: 'receiptNumber',
      key: 'receiptNumber',
    },
    {
      title: t('patients.fullName'),
      dataIndex: 'patientName',
      key: 'patientName',
    },
    {
      title: 'Date',
      dataIndex: 'date',
      key: 'date',
      render: (date: string) => dayjs(date).format('DD/MM/YYYY'),
    },
    {
      title: 'Amount',
      dataIndex: 'amount',
      key: 'amount',
      render: (amount: number) => formatCurrency(amount),
    },
    {
      title: 'Payment Method',
      dataIndex: 'paymentMethod',
      key: 'paymentMethod',
      render: (method: string) => getPaymentMethodText(method),
    },
    {
      title: 'Status',
      dataIndex: 'status',
      key: 'status',
      render: (status: string) => (
        <Tag color={getStatusColor(status)}>
          {status.charAt(0).toUpperCase() + status.slice(1)}
        </Tag>
      ),
    },
    {
      title: t('common.actions'),
      key: 'actions',
      render: (_, record: Receipt) => (
        <Space>
          <Button
            type="primary"
            icon={<EyeOutlined />}
            size="small"
            onClick={() => showReceiptDetail(record)}
          >
            View
          </Button>
          <Button
            icon={<DownloadOutlined />}
            size="small"
          >
            Download
          </Button>
          <Button
            icon={<PrinterOutlined />}
            size="small"
          >
            Print
          </Button>
        </Space>
      ),
    },
  ];

  const filteredReceipts = receipts.filter(receipt =>
    receipt.patientName.toLowerCase().includes(searchText.toLowerCase()) ||
    receipt.receiptNumber.toLowerCase().includes(searchText.toLowerCase())
  );

  return (
    <div>
      <div style={{ marginBottom: 16 }}>
        <h2>{t('menu.receipts')}</h2>
        
        <div style={{ marginBottom: 16, display: 'flex', gap: 16 }}>
          <Input
            placeholder="Search receipts..."
            prefix={<SearchOutlined />}
            value={searchText}
            onChange={(e) => setSearchText(e.target.value)}
            style={{ width: 300 }}
          />
          <RangePicker />
        </div>
      </div>

      <Table
        columns={columns}
        dataSource={filteredReceipts}
        loading={loading}
        rowKey="id"
        pagination={{ pageSize: 10 }}
      />

      <Modal
        title="Receipt Details"
        open={detailModalVisible}
        onCancel={() => setDetailModalVisible(false)}
        footer={[
          <Button key="close" onClick={() => setDetailModalVisible(false)}>
            Close
          </Button>,
          <Button key="download" type="primary" icon={<DownloadOutlined />}>
            Download
          </Button>,
          <Button key="print" icon={<PrinterOutlined />}>
            Print
          </Button>
        ]}
        width={600}
      >
        {selectedReceipt && (
          <div>
            <Card title="Receipt Information" style={{ marginBottom: 16 }}>
              <Descriptions column={2}>
                <Descriptions.Item label="Receipt No.">
                  {selectedReceipt.receiptNumber}
                </Descriptions.Item>
                <Descriptions.Item label="Date">
                  {dayjs(selectedReceipt.date).format('DD/MM/YYYY')}
                </Descriptions.Item>
                <Descriptions.Item label="Patient">
                  {selectedReceipt.patientName}
                </Descriptions.Item>
                <Descriptions.Item label="Payment Method">
                  {getPaymentMethodText(selectedReceipt.paymentMethod)}
                </Descriptions.Item>
                <Descriptions.Item label="Status">
                  <Tag color={getStatusColor(selectedReceipt.status)}>
                    {selectedReceipt.status.charAt(0).toUpperCase() + selectedReceipt.status.slice(1)}
                  </Tag>
                </Descriptions.Item>
              </Descriptions>
            </Card>

            <Card title="Services">
              <Table
                dataSource={selectedReceipt.services}
                columns={[
                  {
                    title: 'Service',
                    dataIndex: 'name',
                    key: 'name',
                  },
                  {
                    title: 'Price',
                    dataIndex: 'price',
                    key: 'price',
                    render: (price: number) => formatCurrency(price),
                  }
                ]}
                pagination={false}
                summary={() => (
                  <Table.Summary.Row>
                    <Table.Summary.Cell index={0}>
                      <strong>Total</strong>
                    </Table.Summary.Cell>
                    <Table.Summary.Cell index={1}>
                      <strong>{formatCurrency(selectedReceipt.amount)}</strong>
                    </Table.Summary.Cell>
                  </Table.Summary.Row>
                )}
              />
            </Card>
          </div>
        )}
      </Modal>
    </div>
  );
};

export default Receipts;