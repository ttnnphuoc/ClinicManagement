import React, { useState, useEffect } from 'react';
import { Table, Button, Form, Modal, message, Space, Tag, Card, Row, Col, Statistic } from 'antd';
import { PlusOutlined, EyeOutlined, EditOutlined, DeleteOutlined, PrinterOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import dayjs from 'dayjs';

interface PrescriptionMedicine {
  medicineId: number;
  medicineName: string;
  dosage: string;
  frequency: string;
  duration: string;
  instructions: string;
  quantity: number;
}

interface Prescription {
  id: number;
  prescriptionNumber: string;
  patientId: number;
  patientName: string;
  doctorId: number;
  doctorName: string;
  appointmentId?: number;
  prescriptionDate: string;
  status: 'pending' | 'dispensed' | 'partial' | 'cancelled';
  medicines: PrescriptionMedicine[];
  notes?: string;
  totalAmount: number;
}

const Prescriptions: React.FC = () => {
  const { t } = useTranslation();
  const [prescriptions, setPrescriptions] = useState<Prescription[]>([]);
  const [loading, setLoading] = useState(false);
  const [detailModalVisible, setDetailModalVisible] = useState(false);
  const [selectedPrescription, setSelectedPrescription] = useState<Prescription | null>(null);

  useEffect(() => {
    fetchPrescriptions();
  }, []);

  const fetchPrescriptions = async () => {
    setLoading(true);
    try {
      // TODO: Replace with actual API call
      const mockData: Prescription[] = [
        {
          id: 1,
          prescriptionNumber: 'PRX-2024-001',
          patientId: 1,
          patientName: 'Nguyen Van A',
          doctorId: 1,
          doctorName: 'Dr. Tran Minh',
          appointmentId: 1,
          prescriptionDate: '2024-01-15',
          status: 'dispensed',
          medicines: [
            {
              medicineId: 1,
              medicineName: 'Paracetamol 500mg',
              dosage: '500mg',
              frequency: '3 times daily',
              duration: '7 days',
              instructions: 'Take after meals',
              quantity: 21
            },
            {
              medicineId: 2,
              medicineName: 'Amoxicillin 250mg',
              dosage: '250mg',
              frequency: '2 times daily',
              duration: '5 days',
              instructions: 'Take before meals',
              quantity: 10
            }
          ],
          notes: 'Patient has mild fever and throat infection',
          totalAmount: 85000
        },
        {
          id: 2,
          prescriptionNumber: 'PRX-2024-002',
          patientId: 2,
          patientName: 'Tran Thi B',
          doctorId: 2,
          doctorName: 'Dr. Le Van C',
          prescriptionDate: '2024-01-16',
          status: 'pending',
          medicines: [
            {
              medicineId: 3,
              medicineName: 'Vitamin D3',
              dosage: '1000 IU',
              frequency: '1 time daily',
              duration: '30 days',
              instructions: 'Take with breakfast',
              quantity: 30
            }
          ],
          notes: 'Vitamin D deficiency',
          totalAmount: 120000
        }
      ];
      setPrescriptions(mockData);
    } catch (error) {
      message.error('Failed to fetch prescriptions');
    } finally {
      setLoading(false);
    }
  };

  const showPrescriptionDetail = (prescription: Prescription) => {
    setSelectedPrescription(prescription);
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
      case 'pending': return 'orange';
      case 'dispensed': return 'green';
      case 'partial': return 'blue';
      case 'cancelled': return 'red';
      default: return 'default';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'pending': return t('prescriptions.pending');
      case 'dispensed': return t('prescriptions.dispensed');
      case 'partial': return t('prescriptions.partial');
      case 'cancelled': return t('prescriptions.cancelled');
      default: return status;
    }
  };

  const columns = [
    {
      title: t('prescriptions.prescriptionNumber'),
      dataIndex: 'prescriptionNumber',
      key: 'prescriptionNumber',
    },
    {
      title: t('patients.fullName'),
      dataIndex: 'patientName',
      key: 'patientName',
    },
    {
      title: t('prescriptions.doctor'),
      dataIndex: 'doctorName',
      key: 'doctorName',
    },
    {
      title: t('prescriptions.date'),
      dataIndex: 'prescriptionDate',
      key: 'prescriptionDate',
      render: (date: string) => dayjs(date).format('DD/MM/YYYY'),
    },
    {
      title: t('prescriptions.medicines'),
      dataIndex: 'medicines',
      key: 'medicines',
      render: (medicines: PrescriptionMedicine[]) => `${medicines.length} ${t('prescriptions.items')}`,
    },
    {
      title: t('prescriptions.totalAmount'),
      dataIndex: 'totalAmount',
      key: 'totalAmount',
      render: (amount: number) => formatCurrency(amount),
    },
    {
      title: t('prescriptions.status'),
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
      render: (_, record: Prescription) => (
        <Space>
          <Button
            type="primary"
            icon={<EyeOutlined />}
            size="small"
            onClick={() => showPrescriptionDetail(record)}
          >
            {t('prescriptions.view')}
          </Button>
          <Button
            icon={<PrinterOutlined />}
            size="small"
          >
            {t('prescriptions.print')}
          </Button>
          {record.status === 'pending' && (
            <Button
              type="default"
              size="small"
            >
              {t('prescriptions.dispense')}
            </Button>
          )}
        </Space>
      ),
    },
  ];

  const totalPrescriptions = prescriptions.length;
  const pendingPrescriptions = prescriptions.filter(p => p.status === 'pending').length;
  const dispensedPrescriptions = prescriptions.filter(p => p.status === 'dispensed').length;
  const totalValue = prescriptions.reduce((sum, p) => sum + p.totalAmount, 0);

  return (
    <div>
      <div style={{ marginBottom: 24 }}>
        <Row gutter={16}>
          <Col span={6}>
            <Card>
              <Statistic
                title={t('prescriptions.totalPrescriptions')}
                value={totalPrescriptions}
                prefix={<PlusOutlined />}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title={t('prescriptions.pending')}
                value={pendingPrescriptions}
                valueStyle={{ color: '#faad14' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title={t('prescriptions.dispensed')}
                value={dispensedPrescriptions}
                valueStyle={{ color: '#52c41a' }}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card>
              <Statistic
                title={t('prescriptions.totalValue')}
                value={totalValue}
                formatter={(value) => formatCurrency(Number(value))}
              />
            </Card>
          </Col>
        </Row>
      </div>

      <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h2>{t('menu.prescriptions')}</h2>
        <Button
          type="primary"
          icon={<PlusOutlined />}
        >
          {t('prescriptions.newPrescription')}
        </Button>
      </div>

      <Table
        columns={columns}
        dataSource={prescriptions}
        loading={loading}
        rowKey="id"
        pagination={{ pageSize: 10 }}
      />

      <Modal
        title={t('prescriptions.prescriptionDetails')}
        open={detailModalVisible}
        onCancel={() => setDetailModalVisible(false)}
        footer={[
          <Button key="close" onClick={() => setDetailModalVisible(false)}>
            {t('common.close')}
          </Button>,
          <Button key="print" type="primary" icon={<PrinterOutlined />}>
            {t('prescriptions.print')}
          </Button>
        ]}
        width={800}
      >
        {selectedPrescription && (
          <div>
            <Card title={t('prescriptions.prescriptionInformation')} style={{ marginBottom: 16 }}>
              <Row gutter={16}>
                <Col span={12}>
                  <p><strong>{t('prescriptions.prescriptionNumber')}:</strong> {selectedPrescription.prescriptionNumber}</p>
                  <p><strong>{t('prescriptions.patient')}:</strong> {selectedPrescription.patientName}</p>
                  <p><strong>{t('prescriptions.doctor')}:</strong> {selectedPrescription.doctorName}</p>
                </Col>
                <Col span={12}>
                  <p><strong>{t('prescriptions.date')}:</strong> {dayjs(selectedPrescription.prescriptionDate).format('DD/MM/YYYY')}</p>
                  <p><strong>{t('prescriptions.status')}:</strong> 
                    <Tag color={getStatusColor(selectedPrescription.status)} style={{ marginLeft: 8 }}>
                      {getStatusText(selectedPrescription.status)}
                    </Tag>
                  </p>
                  <p><strong>{t('prescriptions.totalAmount')}:</strong> {formatCurrency(selectedPrescription.totalAmount)}</p>
                </Col>
              </Row>
              {selectedPrescription.notes && (
                <p><strong>{t('prescriptions.notes')}:</strong> {selectedPrescription.notes}</p>
              )}
            </Card>

            <Card title={t('prescriptions.prescribedMedicines')}>
              <Table
                dataSource={selectedPrescription.medicines}
                columns={[
                  {
                    title: t('prescriptions.medicine'),
                    dataIndex: 'medicineName',
                    key: 'medicineName',
                  },
                  {
                    title: t('prescriptions.dosage'),
                    dataIndex: 'dosage',
                    key: 'dosage',
                  },
                  {
                    title: t('prescriptions.frequency'),
                    dataIndex: 'frequency',
                    key: 'frequency',
                  },
                  {
                    title: t('prescriptions.duration'),
                    dataIndex: 'duration',
                    key: 'duration',
                  },
                  {
                    title: t('prescriptions.quantity'),
                    dataIndex: 'quantity',
                    key: 'quantity',
                  },
                  {
                    title: t('prescriptions.instructions'),
                    dataIndex: 'instructions',
                    key: 'instructions',
                  }
                ]}
                pagination={false}
                size="small"
              />
            </Card>
          </div>
        )}
      </Modal>
    </div>
  );
};

export default Prescriptions;