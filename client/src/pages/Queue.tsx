import React, { useState, useEffect } from 'react';
import { Table, Button, Tag, Space, Card, Statistic, Row, Col, message } from 'antd';
import { UserOutlined, ClockCircleOutlined, CheckOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

interface QueueItem {
  id: number;
  patientName: string;
  phoneNumber: string;
  appointmentTime: string;
  status: 'waiting' | 'in-progress' | 'completed';
  estimatedWaitTime: number;
}

const Queue: React.FC = () => {
  const { t } = useTranslation();
  const [queueData, setQueueData] = useState<QueueItem[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetchQueueData();
  }, []);

  const fetchQueueData = async () => {
    setLoading(true);
    try {
      // TODO: Replace with actual API call
      const mockData: QueueItem[] = [
        {
          id: 1,
          patientName: 'Nguyen Van A',
          phoneNumber: '0123456789',
          appointmentTime: '09:00',
          status: 'waiting',
          estimatedWaitTime: 15
        },
        {
          id: 2,
          patientName: 'Tran Thi B',
          phoneNumber: '0987654321',
          appointmentTime: '09:30',
          status: 'in-progress',
          estimatedWaitTime: 0
        }
      ];
      setQueueData(mockData);
    } catch (error) {
      message.error('Failed to fetch queue data');
    } finally {
      setLoading(false);
    }
  };

  const handleStatusChange = (id: number, newStatus: 'waiting' | 'in-progress' | 'completed') => {
    setQueueData(prev => 
      prev.map(item => 
        item.id === id ? { ...item, status: newStatus } : item
      )
    );
    message.success('Status updated successfully');
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'waiting': return 'orange';
      case 'in-progress': return 'blue';
      case 'completed': return 'green';
      default: return 'default';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'waiting': return t('appointments.scheduled');
      case 'in-progress': return 'In Progress';
      case 'completed': return t('appointments.completed');
      default: return status;
    }
  };

  const columns = [
    {
      title: t('patients.fullName'),
      dataIndex: 'patientName',
      key: 'patientName',
    },
    {
      title: t('patients.phoneNumber'),
      dataIndex: 'phoneNumber',
      key: 'phoneNumber',
    },
    {
      title: t('appointments.appointmentDate'),
      dataIndex: 'appointmentTime',
      key: 'appointmentTime',
    },
    {
      title: t('appointments.status'),
      dataIndex: 'status',
      key: 'status',
      render: (status: string) => (
        <Tag color={getStatusColor(status)}>
          {getStatusText(status)}
        </Tag>
      ),
    },
    {
      title: 'Estimated Wait (min)',
      dataIndex: 'estimatedWaitTime',
      key: 'estimatedWaitTime',
      render: (time: number) => time > 0 ? `${time} min` : '-',
    },
    {
      title: t('common.actions'),
      key: 'actions',
      render: (_, record: QueueItem) => (
        <Space>
          {record.status === 'waiting' && (
            <Button 
              type="primary" 
              size="small"
              onClick={() => handleStatusChange(record.id, 'in-progress')}
            >
              Start
            </Button>
          )}
          {record.status === 'in-progress' && (
            <Button 
              type="primary" 
              size="small"
              onClick={() => handleStatusChange(record.id, 'completed')}
            >
              Complete
            </Button>
          )}
        </Space>
      ),
    },
  ];

  const waitingCount = queueData.filter(item => item.status === 'waiting').length;
  const inProgressCount = queueData.filter(item => item.status === 'in-progress').length;
  const completedCount = queueData.filter(item => item.status === 'completed').length;

  return (
    <div>
      <Row gutter={16} style={{ marginBottom: 24 }}>
        <Col span={6}>
          <Card>
            <Statistic
              title="Waiting"
              value={waitingCount}
              prefix={<ClockCircleOutlined />}
              valueStyle={{ color: '#faad14' }}
            />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="In Progress"
              value={inProgressCount}
              prefix={<UserOutlined />}
              valueStyle={{ color: '#1890ff' }}
            />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="Completed"
              value={completedCount}
              prefix={<CheckOutlined />}
              valueStyle={{ color: '#52c41a' }}
            />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="Total Today"
              value={queueData.length}
              prefix={<UserOutlined />}
            />
          </Card>
        </Col>
      </Row>

      <Table
        columns={columns}
        dataSource={queueData}
        loading={loading}
        rowKey="id"
        pagination={false}
      />
    </div>
  );
};

export default Queue;