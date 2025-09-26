import React, { useState, useEffect } from 'react';
import { Table, Button, Form, Input, Modal, message, Space, Tag, Select } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

interface Room {
  id: number;
  name: string;
  type: string;
  capacity: number;
  status: 'available' | 'occupied' | 'maintenance';
  equipment: string[];
  location: string;
}

const Rooms: React.FC = () => {
  const { t } = useTranslation();
  const [rooms, setRooms] = useState<Room[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalVisible, setModalVisible] = useState(false);
  const [editingRoom, setEditingRoom] = useState<Room | null>(null);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchRooms();
  }, []);

  const fetchRooms = async () => {
    setLoading(true);
    try {
      // TODO: Replace with actual API call
      const mockData: Room[] = [
        {
          id: 1,
          name: 'Room 101',
          type: 'Consultation',
          capacity: 2,
          status: 'available',
          equipment: ['Examination Table', 'Blood Pressure Monitor'],
          location: 'Floor 1'
        },
        {
          id: 2,
          name: 'Room 201',
          type: 'Surgery',
          capacity: 5,
          status: 'occupied',
          equipment: ['Operating Table', 'Anesthesia Machine', 'Heart Monitor'],
          location: 'Floor 2'
        }
      ];
      setRooms(mockData);
    } catch (error) {
      message.error('Failed to fetch rooms');
    } finally {
      setLoading(false);
    }
  };

  const showModal = (room?: Room) => {
    setEditingRoom(room || null);
    if (room) {
      form.setFieldsValue(room);
    } else {
      form.resetFields();
    }
    setModalVisible(true);
  };

  const handleOk = async () => {
    try {
      const values = await form.validateFields();
      if (editingRoom) {
        // Update room
        setRooms(prev => 
          prev.map(room => 
            room.id === editingRoom.id ? { ...editingRoom, ...values } : room
          )
        );
        message.success('Room updated successfully');
      } else {
        // Add new room
        const newRoom: Room = {
          id: Date.now(),
          ...values
        };
        setRooms(prev => [...prev, newRoom]);
        message.success('Room created successfully');
      }
      setModalVisible(false);
      form.resetFields();
    } catch (error) {
      console.error('Validation failed:', error);
    }
  };

  const handleDelete = (id: number) => {
    Modal.confirm({
      title: 'Are you sure you want to delete this room?',
      onOk: () => {
        setRooms(prev => prev.filter(room => room.id !== id));
        message.success('Room deleted successfully');
      }
    });
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'available': return 'green';
      case 'occupied': return 'red';
      case 'maintenance': return 'orange';
      default: return 'default';
    }
  };

  const columns = [
    {
      title: 'Room Name',
      dataIndex: 'name',
      key: 'name',
    },
    {
      title: 'Type',
      dataIndex: 'type',
      key: 'type',
    },
    {
      title: 'Capacity',
      dataIndex: 'capacity',
      key: 'capacity',
    },
    {
      title: 'Location',
      dataIndex: 'location',
      key: 'location',
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
      title: 'Equipment',
      dataIndex: 'equipment',
      key: 'equipment',
      render: (equipment: string[]) => equipment.join(', '),
    },
    {
      title: t('common.actions'),
      key: 'actions',
      render: (_, record: Room) => (
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
      <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between' }}>
        <h2>{t('menu.rooms')}</h2>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => showModal()}
        >
          Add Room
        </Button>
      </div>

      <Table
        columns={columns}
        dataSource={rooms}
        loading={loading}
        rowKey="id"
        pagination={{ pageSize: 10 }}
      />

      <Modal
        title={editingRoom ? 'Edit Room' : 'Add Room'}
        open={modalVisible}
        onOk={handleOk}
        onCancel={() => setModalVisible(false)}
        width={600}
      >
        <Form form={form} layout="vertical">
          <Form.Item
            label="Room Name"
            name="name"
            rules={[{ required: true, message: 'Room name is required' }]}
          >
            <Input placeholder="Enter room name" />
          </Form.Item>

          <Form.Item
            label="Type"
            name="type"
            rules={[{ required: true, message: 'Room type is required' }]}
          >
            <Select placeholder="Select room type">
              <Select.Option value="Consultation">Consultation</Select.Option>
              <Select.Option value="Surgery">Surgery</Select.Option>
              <Select.Option value="Emergency">Emergency</Select.Option>
              <Select.Option value="Laboratory">Laboratory</Select.Option>
              <Select.Option value="Radiology">Radiology</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item
            label="Capacity"
            name="capacity"
            rules={[{ required: true, message: 'Capacity is required' }]}
          >
            <Input type="number" placeholder="Enter room capacity" />
          </Form.Item>

          <Form.Item
            label="Location"
            name="location"
            rules={[{ required: true, message: 'Location is required' }]}
          >
            <Input placeholder="Enter location (e.g., Floor 1)" />
          </Form.Item>

          <Form.Item
            label="Status"
            name="status"
            rules={[{ required: true, message: 'Status is required' }]}
            initialValue="available"
          >
            <Select>
              <Select.Option value="available">Available</Select.Option>
              <Select.Option value="occupied">Occupied</Select.Option>
              <Select.Option value="maintenance">Maintenance</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item
            label="Equipment"
            name="equipment"
          >
            <Select
              mode="tags"
              placeholder="Enter equipment (press Enter to add)"
              style={{ width: '100%' }}
            >
              <Select.Option value="Examination Table">Examination Table</Select.Option>
              <Select.Option value="Blood Pressure Monitor">Blood Pressure Monitor</Select.Option>
              <Select.Option value="Stethoscope">Stethoscope</Select.Option>
              <Select.Option value="Operating Table">Operating Table</Select.Option>
              <Select.Option value="Anesthesia Machine">Anesthesia Machine</Select.Option>
              <Select.Option value="Heart Monitor">Heart Monitor</Select.Option>
            </Select>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default Rooms;