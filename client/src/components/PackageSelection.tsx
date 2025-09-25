import React, { useState, useEffect } from 'react';
import { Card, Button, Tag, List, message, Spin, Row, Col } from 'antd';
import { CheckCircleOutlined, CrownOutlined, GiftOutlined, StarOutlined } from '@ant-design/icons';
import { subscriptionService, type SubscriptionPackage } from '../services/subscriptionService';

interface PackageSelectionProps {
  onPackageSelect: (packageId: string) => void;
  selectedPackageId?: string;
  showSelectButton?: boolean;
  loading?: boolean;
}

const PackageSelection: React.FC<PackageSelectionProps> = ({
  onPackageSelect,
  selectedPackageId,
  showSelectButton = true,
  loading = false,
}) => {
  const [packages, setPackages] = useState<SubscriptionPackage[]>([]);
  const [loadingPackages, setLoadingPackages] = useState(true);

  useEffect(() => {
    loadPackages();
  }, []);

  const loadPackages = async () => {
    try {
      const response = await subscriptionService.getPackages();
      if (response.success) {
        setPackages(response.data);
      } else {
        message.error('Failed to load packages');
      }
    } catch (error) {
      message.error('Error loading packages');
    } finally {
      setLoadingPackages(false);
    }
  };

  const getPackageIcon = (packageName: string) => {
    switch (packageName.toLowerCase()) {
      case 'trial':
        return <GiftOutlined style={{ fontSize: '24px', color: '#52c41a' }} />;
      case 'basic':
        return <CheckCircleOutlined style={{ fontSize: '24px', color: '#1890ff' }} />;
      case 'professional':
        return <StarOutlined style={{ fontSize: '24px', color: '#722ed1' }} />;
      case 'premium':
        return <CrownOutlined style={{ fontSize: '24px', color: '#fa8c16' }} />;
      default:
        return <CheckCircleOutlined style={{ fontSize: '24px', color: '#1890ff' }} />;
    }
  };

  const getPackageColor = (packageName: string) => {
    switch (packageName.toLowerCase()) {
      case 'trial':
        return '#52c41a';
      case 'basic':
        return '#1890ff';
      case 'professional':
        return '#722ed1';
      case 'premium':
        return '#fa8c16';
      default:
        return '#1890ff';
    }
  };

  const formatPrice = (price: number) => {
    return price === 0 ? 'Free' : `$${price.toFixed(2)}/month`;
  };

  if (loadingPackages) {
    return (
      <div style={{ textAlign: 'center', padding: '50px' }}>
        <Spin size="large" />
      </div>
    );
  }

  return (
    <div>
      <Row gutter={[16, 16]}>
        {packages.map((pkg) => (
          <Col xs={24} sm={12} lg={6} key={pkg.id}>
            <Card
              hoverable
              className={selectedPackageId === pkg.id ? 'border-2 border-blue-500' : ''}
              style={{
                height: '100%',
                borderColor: selectedPackageId === pkg.id ? getPackageColor(pkg.name) : undefined,
              }}
              actions={
                showSelectButton
                  ? [
                      <Button
                        key="select"
                        type={selectedPackageId === pkg.id ? 'primary' : 'default'}
                        onClick={() => onPackageSelect(pkg.id)}
                        loading={loading && selectedPackageId === pkg.id}
                        block
                      >
                        {selectedPackageId === pkg.id ? 'Selected' : 'Select'}
                      </Button>,
                    ]
                  : []
              }
            >
              <div style={{ textAlign: 'center', marginBottom: '16px' }}>
                {getPackageIcon(pkg.name)}
                <div style={{ marginTop: '8px' }}>
                  <h3 style={{ margin: '8px 0', color: getPackageColor(pkg.name) }}>
                    {pkg.name}
                  </h3>
                  {pkg.isTrialPackage && (
                    <Tag color="green" style={{ marginBottom: '8px' }}>
                      Free Trial
                    </Tag>
                  )}
                  <div style={{ fontSize: '24px', fontWeight: 'bold', margin: '8px 0' }}>
                    {formatPrice(pkg.price)}
                  </div>
                  <p style={{ color: '#666', fontSize: '14px' }}>{pkg.description}</p>
                </div>
              </div>

              <List
                size="small"
                dataSource={pkg.packageLimits}
                renderItem={(limit) => (
                  <List.Item style={{ padding: '4px 0', border: 'none' }}>
                    <div style={{ fontSize: '12px' }}>
                      <CheckCircleOutlined
                        style={{ color: '#52c41a', marginRight: '8px' }}
                      />
                      {limit.displayText}
                    </div>
                  </List.Item>
                )}
              />
            </Card>
          </Col>
        ))}
      </Row>
    </div>
  );
};

export default PackageSelection;