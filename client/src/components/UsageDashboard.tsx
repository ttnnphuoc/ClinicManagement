import React, { useState, useEffect } from 'react';
import { Card, Progress, Row, Col, Statistic, Tag, Typography, Alert } from 'antd';
import { 
  ShopOutlined, 
  UserOutlined, 
  TeamOutlined, 
  CalendarOutlined,
  TrophyOutlined
} from '@ant-design/icons';
import { subscriptionService, type UsageTracking, type Subscription } from '../services/subscriptionService';

const { Text, Title } = Typography;

interface UsageDashboardProps {
  className?: string;
  style?: React.CSSProperties;
}

const UsageDashboard: React.FC<UsageDashboardProps> = ({ className, style }) => {
  const [usage, setUsage] = useState<UsageTracking[]>([]);
  const [subscription, setSubscription] = useState<Subscription | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadUsageData();
  }, []);

  const loadUsageData = async () => {
    try {
      setLoading(true);
      const [usageResponse, subResponse] = await Promise.all([
        subscriptionService.getUsage().catch(() => ({ success: false, data: [] })),
        subscriptionService.getCurrentSubscription().catch(() => ({ success: false, data: null })),
      ]);

      if (usageResponse.success) {
        setUsage(usageResponse.data);
      }

      if (subResponse.success) {
        setSubscription(subResponse.data);
      }
    } catch (error) {
      console.error('Failed to load usage data:', error);
    } finally {
      setLoading(false);
    }
  };

  const getUsagePercentage = (usageItem: UsageTracking): number => {
    if (!usageItem.limit || usageItem.limit === -1) return 0;
    return Math.min((usageItem.currentUsage / usageItem.limit) * 100, 100);
  };

  const getUsageStatus = (percentage: number): 'success' | 'normal' | 'exception' => {
    if (percentage >= 90) return 'exception';
    if (percentage >= 75) return 'normal';
    return 'success';
  };

  const getResourceIcon = (resourceType: string) => {
    switch (resourceType) {
      case 'Clinics':
        return <ShopOutlined style={{ fontSize: '24px' }} />;
      case 'Patients':
        return <UserOutlined style={{ fontSize: '24px' }} />;
      case 'Staff':
        return <TeamOutlined style={{ fontSize: '24px' }} />;
      case 'Appointments':
        return <CalendarOutlined style={{ fontSize: '24px' }} />;
      default:
        return <TrophyOutlined style={{ fontSize: '24px' }} />;
    }
  };

  const getResourceColor = (resourceType: string, percentage: number) => {
    if (percentage >= 90) return '#ff4d4f';
    if (percentage >= 75) return '#fa8c16';
    
    switch (resourceType) {
      case 'Clinics':
        return '#722ed1';
      case 'Patients':
        return '#1890ff';
      case 'Staff':
        return '#52c41a';
      case 'Appointments':
        return '#fa541c';
      default:
        return '#1890ff';
    }
  };

  if (!subscription) {
    return (
      <Card className={className} style={style}>
        <div style={{ textAlign: 'center', padding: '20px' }}>
          <TrophyOutlined style={{ fontSize: '48px', color: '#ccc', marginBottom: '16px' }} />
          <Title level={4}>No Active Subscription</Title>
          <Text type="secondary">
            Subscribe to a package to view your usage statistics.
          </Text>
        </div>
      </Card>
    );
  }

  const nearLimitUsage = usage.filter(u => 
    u.limit && u.limit !== -1 && getUsagePercentage(u) >= 75
  );

  return (
    <div className={className} style={style}>
      {nearLimitUsage.length > 0 && (
        <Alert
          message="Usage Warning"
          description={`You are approaching the limit for: ${nearLimitUsage.map(u => u.resourceType).join(', ')}`}
          type="warning"
          showIcon
          style={{ marginBottom: '16px' }}
          closable
        />
      )}

      <Card
        title={
          <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <TrophyOutlined />
            <span>Current Plan: {subscription.subscriptionPackage.name}</span>
            <Tag color="blue">{subscription.status}</Tag>
          </div>
        }
        loading={loading}
      >
        <Row gutter={[16, 16]}>
          {usage.map((usageItem) => {
            const percentage = getUsagePercentage(usageItem);
            const color = getResourceColor(usageItem.resourceType, percentage);

            return (
              <Col xs={12} sm={6} key={usageItem.resourceType}>
                <Card
                  size="small"
                  style={{ 
                    height: '140px',
                    border: percentage >= 90 ? '2px solid #ff4d4f' : undefined 
                  }}
                >
                  <div style={{ textAlign: 'center' }}>
                    <div style={{ color, marginBottom: '8px' }}>
                      {getResourceIcon(usageItem.resourceType)}
                    </div>
                    
                    <Statistic
                      title={usageItem.resourceType}
                      value={usageItem.currentUsage}
                      suffix={
                        usageItem.limit && usageItem.limit !== -1 
                          ? `/ ${usageItem.limit}` 
                          : '/ ∞'
                      }
                      valueStyle={{ 
                        fontSize: '18px',
                        color: percentage >= 90 ? '#ff4d4f' : undefined
                      }}
                    />
                    
                    {usageItem.limit && usageItem.limit !== -1 ? (
                      <Progress
                        percent={percentage}
                        status={getUsageStatus(percentage)}
                        showInfo={false}
                        size="small"
                        style={{ marginTop: '8px' }}
                      />
                    ) : (
                      <div style={{ marginTop: '8px', height: '6px' }}>
                        <Text type="secondary" style={{ fontSize: '12px' }}>
                          Unlimited
                        </Text>
                      </div>
                    )}
                  </div>
                </Card>
              </Col>
            );
          })}
        </Row>
      </Card>
    </div>
  );
};

export default UsageDashboard;