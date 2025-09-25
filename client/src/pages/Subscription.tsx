import React, { useState, useEffect } from 'react';
import { 
  Card, 
  Button, 
  Typography, 
  Progress, 
  Row, 
  Col, 
  Statistic, 
  Tag, 
  message, 
  Modal, 
  Space,
  Descriptions,
  Alert
} from 'antd';
import { 
  CrownOutlined, 
  CalendarOutlined, 
  TrophyOutlined, 
  ArrowUpOutlined,
  StopOutlined
} from '@ant-design/icons';
import { subscriptionService, type Subscription, type UsageTracking } from '../services/subscriptionService';
import PackageSelection from '../components/PackageSelection';
import dayjs from 'dayjs';

const { Title, Text } = Typography;

const SubscriptionPage: React.FC = () => {
  const [subscription, setSubscription] = useState<Subscription | null>(null);
  const [usage, setUsage] = useState<UsageTracking[]>([]);
  const [loading, setLoading] = useState(true);
  const [upgradeModalVisible, setUpgradeModalVisible] = useState(false);
  const [upgrading, setUpgrading] = useState(false);
  const [selectedUpgradePackage, setSelectedUpgradePackage] = useState<string>('');

  useEffect(() => {
    loadSubscriptionData();
  }, []);

  const loadSubscriptionData = async () => {
    try {
      setLoading(true);
      const [subResponse, usageResponse] = await Promise.all([
        subscriptionService.getCurrentSubscription(),
        subscriptionService.getUsage(),
      ]);

      if (subResponse.success) {
        setSubscription(subResponse.data);
      }

      if (usageResponse.success) {
        setUsage(usageResponse.data);
      }
    } catch (error: any) {
      if (error.response?.status !== 404) {
        message.error('Failed to load subscription data');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleUpgrade = async () => {
    if (!selectedUpgradePackage) {
      message.error('Please select a package to upgrade to');
      return;
    }

    try {
      setUpgrading(true);
      const response = await subscriptionService.upgrade({
        newPackageId: selectedUpgradePackage,
      });

      if (response.success) {
        message.success('Subscription upgraded successfully!');
        setUpgradeModalVisible(false);
        await loadSubscriptionData();
      } else {
        message.error('Failed to upgrade subscription');
      }
    } catch (error) {
      message.error('Error upgrading subscription');
    } finally {
      setUpgrading(false);
    }
  };

  const handleCancelSubscription = () => {
    Modal.confirm({
      title: 'Cancel Subscription',
      content: 'Are you sure you want to cancel your subscription? This will disable automatic renewal.',
      onOk: async () => {
        try {
          const response = await subscriptionService.cancel();
          if (response.success) {
            message.success('Subscription cancelled successfully');
            await loadSubscriptionData();
          } else {
            message.error('Failed to cancel subscription');
          }
        } catch (error) {
          message.error('Error cancelling subscription');
        }
      },
    });
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

  const getDaysUntilExpiry = (): number => {
    if (!subscription) return 0;
    return dayjs(subscription.endDate).diff(dayjs(), 'day');
  };

  if (loading) {
    return (
      <div style={{ padding: '24px' }}>
        <Card loading />
      </div>
    );
  }

  if (!subscription) {
    return (
      <div style={{ padding: '24px' }}>
        <Card>
          <div style={{ textAlign: 'center', padding: '50px 0' }}>
            <CrownOutlined style={{ fontSize: '64px', color: '#ccc', marginBottom: '16px' }} />
            <Title level={3}>No Active Subscription</Title>
            <Text type="secondary">
              You don't have an active subscription. Create your first clinic to get started.
            </Text>
          </div>
        </Card>
      </div>
    );
  }

  const daysUntilExpiry = getDaysUntilExpiry();
  const isExpiringSoon = daysUntilExpiry <= 7;

  return (
    <div style={{ padding: '24px' }}>
      <Title level={2}>
        <CrownOutlined /> Subscription Management
      </Title>

      {isExpiringSoon && (
        <Alert
          message="Subscription Expiring Soon"
          description={`Your subscription will expire in ${daysUntilExpiry} day${daysUntilExpiry === 1 ? '' : 's'}. Please renew to avoid service interruption.`}
          type="warning"
          showIcon
          style={{ marginBottom: '24px' }}
        />
      )}

      <Row gutter={[16, 16]}>
        {/* Current Subscription Info */}
        <Col xs={24} lg={12}>
          <Card
            title={
              <Space>
                <TrophyOutlined />
                Current Subscription
              </Space>
            }
            extra={
              <Tag color="blue" style={{ fontSize: '12px' }}>
                {subscription.status}
              </Tag>
            }
          >
            <Descriptions column={1} size="small">
              <Descriptions.Item label="Package">
                <Text strong style={{ fontSize: '16px' }}>
                  {subscription.subscriptionPackage.name}
                </Text>
              </Descriptions.Item>
              <Descriptions.Item label="Price">
                <Text strong>
                  ${subscription.subscriptionPackage.price}/month
                </Text>
              </Descriptions.Item>
              <Descriptions.Item label="Start Date">
                {dayjs(subscription.startDate).format('MMM DD, YYYY')}
              </Descriptions.Item>
              <Descriptions.Item label="End Date">
                {dayjs(subscription.endDate).format('MMM DD, YYYY')}
              </Descriptions.Item>
              <Descriptions.Item label="Auto Renew">
                <Tag color={subscription.autoRenew ? 'green' : 'red'}>
                  {subscription.autoRenew ? 'Enabled' : 'Disabled'}
                </Tag>
              </Descriptions.Item>
              {subscription.lastPaymentDate && (
                <Descriptions.Item label="Last Payment">
                  {dayjs(subscription.lastPaymentDate).format('MMM DD, YYYY')}
                </Descriptions.Item>
              )}
            </Descriptions>

            <div style={{ marginTop: '16px' }}>
              <Space>
                <Button
                  type="primary"
                  icon={<ArrowUpOutlined />}
                  onClick={() => setUpgradeModalVisible(true)}
                >
                  Upgrade Plan
                </Button>
                <Button
                  icon={<StopOutlined />}
                  onClick={handleCancelSubscription}
                  danger
                >
                  Cancel Auto-Renewal
                </Button>
              </Space>
            </div>
          </Card>
        </Col>

        {/* Usage Statistics */}
        <Col xs={24} lg={12}>
          <Card
            title={
              <Space>
                <CalendarOutlined />
                Usage Statistics
              </Space>
            }
          >
            <Row gutter={[16, 16]}>
              {usage.map((usageItem) => (
                <Col span={12} key={usageItem.resourceType}>
                  <div style={{ marginBottom: '16px' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '4px' }}>
                      <Text>{usageItem.resourceType}</Text>
                      <Text type="secondary">
                        {usageItem.currentUsage}
                        {usageItem.limit && usageItem.limit !== -1 ? `/${usageItem.limit}` : ''}
                      </Text>
                    </div>
                    {usageItem.limit && usageItem.limit !== -1 ? (
                      <Progress
                        percent={getUsagePercentage(usageItem)}
                        status={getUsageStatus(getUsagePercentage(usageItem))}
                        showInfo={false}
                        size="small"
                      />
                    ) : (
                      <Progress
                        percent={0}
                        status="success"
                        showInfo={false}
                        size="small"
                        format={() => 'Unlimited'}
                      />
                    )}
                  </div>
                </Col>
              ))}
            </Row>
          </Card>
        </Col>
      </Row>

      {/* Upgrade Modal */}
      <Modal
        title="Upgrade Your Subscription"
        open={upgradeModalVisible}
        onCancel={() => {
          setUpgradeModalVisible(false);
          setSelectedUpgradePackage('');
        }}
        footer={[
          <Button
            key="cancel"
            onClick={() => {
              setUpgradeModalVisible(false);
              setSelectedUpgradePackage('');
            }}
          >
            Cancel
          </Button>,
          <Button
            key="upgrade"
            type="primary"
            onClick={handleUpgrade}
            loading={upgrading}
            disabled={!selectedUpgradePackage}
          >
            Upgrade Now
          </Button>,
        ]}
        width={1000}
      >
        <div style={{ marginBottom: '16px' }}>
          <Text type="secondary">
            Choose a new plan to upgrade your subscription. You will be charged prorated amount for the upgrade.
          </Text>
        </div>
        <PackageSelection
          onPackageSelect={setSelectedUpgradePackage}
          selectedPackageId={selectedUpgradePackage}
          showSelectButton={false}
        />
      </Modal>
    </div>
  );
};

export default SubscriptionPage;