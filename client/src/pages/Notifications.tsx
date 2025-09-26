import React, { useState, useEffect } from 'react';
import { List, Badge, Button, Tag, Space, Card, Tabs, Dropdown } from 'antd';
import type { MenuProps } from 'antd';
import { 
  BellOutlined, 
  CheckOutlined, 
  DeleteOutlined, 
  EyeOutlined,
  MoreOutlined,
  NotificationOutlined,
  CalendarOutlined,
  WarningOutlined,
  InfoCircleOutlined
} from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import dayjs from 'dayjs';

interface Notification {
  id: number;
  title: string;
  message: string;
  type: 'info' | 'warning' | 'error' | 'appointment' | 'system';
  read: boolean;
  createdAt: string;
  priority: 'low' | 'medium' | 'high';
}

const Notifications: React.FC = () => {
  const { t } = useTranslation();
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(false);
  const [activeTab, setActiveTab] = useState('all');

  useEffect(() => {
    fetchNotifications();
  }, []);

  const fetchNotifications = async () => {
    setLoading(true);
    try {
      // TODO: Replace with actual API call
      const mockData: Notification[] = [
        {
          id: 1,
          title: 'New Appointment Scheduled',
          message: 'Patient John Doe has scheduled an appointment for tomorrow at 2:00 PM',
          type: 'appointment',
          read: false,
          createdAt: '2024-01-15T10:30:00Z',
          priority: 'medium'
        },
        {
          id: 2,
          title: 'Medicine Stock Low',
          message: 'Paracetamol stock is running low. Only 5 units remaining.',
          type: 'warning',
          read: false,
          createdAt: '2024-01-15T09:15:00Z',
          priority: 'high'
        },
        {
          id: 3,
          title: 'System Update',
          message: 'System maintenance will be performed tonight from 11 PM to 1 AM',
          type: 'system',
          read: true,
          createdAt: '2024-01-14T16:00:00Z',
          priority: 'low'
        }
      ];
      setNotifications(mockData);
    } catch (error) {
      console.error('Failed to fetch notifications:', error);
    } finally {
      setLoading(false);
    }
  };

  const markAsRead = (id: number) => {
    setNotifications(prev =>
      prev.map(notification =>
        notification.id === id ? { ...notification, read: true } : notification
      )
    );
  };

  const markAllAsRead = () => {
    setNotifications(prev =>
      prev.map(notification => ({ ...notification, read: true }))
    );
  };

  const deleteNotification = (id: number) => {
    setNotifications(prev => prev.filter(notification => notification.id !== id));
  };

  const getTypeIcon = (type: string) => {
    switch (type) {
      case 'appointment': return <CalendarOutlined />;
      case 'warning': return <WarningOutlined />;
      case 'error': return <WarningOutlined />;
      case 'system': return <NotificationOutlined />;
      default: return <InfoCircleOutlined />;
    }
  };

  const getTypeColor = (type: string) => {
    switch (type) {
      case 'appointment': return '#1890ff';
      case 'warning': return '#faad14';
      case 'error': return '#ff4d4f';
      case 'system': return '#52c41a';
      default: return '#1890ff';
    }
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'high': return 'red';
      case 'medium': return 'orange';
      case 'low': return 'green';
      default: return 'default';
    }
  };

  const getMenuItems = (notification: Notification): MenuProps['items'] => [
    {
      key: 'read',
      icon: <EyeOutlined />,
      label: notification.read ? 'Mark as Unread' : 'Mark as Read',
      onClick: () => markAsRead(notification.id)
    },
    {
      key: 'delete',
      icon: <DeleteOutlined />,
      label: 'Delete',
      onClick: () => deleteNotification(notification.id),
      danger: true
    }
  ];

  const filterNotifications = (notifications: Notification[], filter: string) => {
    switch (filter) {
      case 'unread':
        return notifications.filter(n => !n.read);
      case 'read':
        return notifications.filter(n => n.read);
      default:
        return notifications;
    }
  };

  const unreadCount = notifications.filter(n => !n.read).length;

  const tabItems = [
    {
      key: 'all',
      label: `All (${notifications.length})`,
      children: (
        <List
          dataSource={filterNotifications(notifications, 'all')}
          loading={loading}
          renderItem={(notification) => (
            <List.Item
              style={{
                backgroundColor: !notification.read ? '#f6f9ff' : 'white',
                border: '1px solid #f0f0f0',
                marginBottom: 8,
                borderRadius: 6,
                padding: '12px 16px'
              }}
            >
              <List.Item.Meta
                avatar={
                  <Badge dot={!notification.read}>
                    <div style={{ 
                      color: getTypeColor(notification.type), 
                      fontSize: 20,
                      display: 'flex',
                      alignItems: 'center'
                    }}>
                      {getTypeIcon(notification.type)}
                    </div>
                  </Badge>
                }
                title={
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <span style={{ fontWeight: !notification.read ? 600 : 400 }}>
                      {notification.title}
                    </span>
                    <Space>
                      <Tag color={getPriorityColor(notification.priority)}>
                        {notification.priority.toUpperCase()}
                      </Tag>
                      <span style={{ fontSize: 12, color: '#999' }}>
                        {dayjs(notification.createdAt).fromNow()}
                      </span>
                    </Space>
                  </div>
                }
                description={notification.message}
              />
              <Dropdown
                menu={{ items: getMenuItems(notification) }}
                trigger={['click']}
              >
                <Button type="text" icon={<MoreOutlined />} size="small" />
              </Dropdown>
            </List.Item>
          )}
        />
      )
    },
    {
      key: 'unread',
      label: `Unread (${unreadCount})`,
      children: (
        <List
          dataSource={filterNotifications(notifications, 'unread')}
          loading={loading}
          renderItem={(notification) => (
            <List.Item
              style={{
                backgroundColor: '#f6f9ff',
                border: '1px solid #f0f0f0',
                marginBottom: 8,
                borderRadius: 6,
                padding: '12px 16px'
              }}
            >
              <List.Item.Meta
                avatar={
                  <Badge dot>
                    <div style={{ 
                      color: getTypeColor(notification.type), 
                      fontSize: 20,
                      display: 'flex',
                      alignItems: 'center'
                    }}>
                      {getTypeIcon(notification.type)}
                    </div>
                  </Badge>
                }
                title={
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <span style={{ fontWeight: 600 }}>
                      {notification.title}
                    </span>
                    <Space>
                      <Tag color={getPriorityColor(notification.priority)}>
                        {notification.priority.toUpperCase()}
                      </Tag>
                      <span style={{ fontSize: 12, color: '#999' }}>
                        {dayjs(notification.createdAt).fromNow()}
                      </span>
                    </Space>
                  </div>
                }
                description={notification.message}
              />
              <Dropdown
                menu={{ items: getMenuItems(notification) }}
                trigger={['click']}
              >
                <Button type="text" icon={<MoreOutlined />} size="small" />
              </Dropdown>
            </List.Item>
          )}
        />
      )
    },
    {
      key: 'read',
      label: `Read (${notifications.length - unreadCount})`,
      children: (
        <List
          dataSource={filterNotifications(notifications, 'read')}
          loading={loading}
          renderItem={(notification) => (
            <List.Item
              style={{
                backgroundColor: 'white',
                border: '1px solid #f0f0f0',
                marginBottom: 8,
                borderRadius: 6,
                padding: '12px 16px'
              }}
            >
              <List.Item.Meta
                avatar={
                  <div style={{ 
                    color: getTypeColor(notification.type), 
                    fontSize: 20,
                    display: 'flex',
                    alignItems: 'center'
                  }}>
                    {getTypeIcon(notification.type)}
                  </div>
                }
                title={
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <span>{notification.title}</span>
                    <Space>
                      <Tag color={getPriorityColor(notification.priority)}>
                        {notification.priority.toUpperCase()}
                      </Tag>
                      <span style={{ fontSize: 12, color: '#999' }}>
                        {dayjs(notification.createdAt).fromNow()}
                      </span>
                    </Space>
                  </div>
                }
                description={notification.message}
              />
              <Dropdown
                menu={{ items: getMenuItems(notification) }}
                trigger={['click']}
              >
                <Button type="text" icon={<MoreOutlined />} size="small" />
              </Dropdown>
            </List.Item>
          )}
        />
      )
    }
  ];

  return (
    <div>
      <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h2>{t('menu.notifications')}</h2>
        <Space>
          {unreadCount > 0 && (
            <Button 
              type="primary" 
              icon={<CheckOutlined />}
              onClick={markAllAsRead}
            >
              Mark All as Read
            </Button>
          )}
        </Space>
      </div>

      <Card>
        <Tabs 
          items={tabItems}
          activeKey={activeTab}
          onChange={setActiveTab}
        />
      </Card>
    </div>
  );
};

export default Notifications;