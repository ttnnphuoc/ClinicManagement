import { useState, useEffect } from 'react';
import { Table, Button, Input, Space, Modal, message, Popconfirm } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, SearchOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { patientService, Patient } from '../services/patientService';
import PatientForm from '../components/PatientForm';

const Patients = () => {
  const { t } = useTranslation();
  const [patients, setPatients] = useState<Patient[]>([]);
  const [loading, setLoading] = useState(false);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [search, setSearch] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingPatient, setEditingPatient] = useState<Patient | null>(null);

  const fetchPatients = async () => {
    setLoading(true);
    try {
      const response = await patientService.getPatients(search, page, pageSize);
      if (response.success && response.data) {
        setPatients(response.data.items);
        setTotal(response.data.total);
      }
    } catch (error: any) {
      message.error(t('errors.UNKNOWN_ERROR'));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPatients();
  }, [page, pageSize, search]);

  const handleSearch = (value: string) => {
    setSearch(value);
    setPage(1);
  };

  const handleAdd = () => {
    setEditingPatient(null);
    setIsModalOpen(true);
  };

  const handleEdit = (patient: Patient) => {
    setEditingPatient(patient);
    setIsModalOpen(true);
  };

  const handleDelete = async (id: string) => {
    try {
      const response = await patientService.deletePatient(id);
      if (response.success) {
        message.success(t('patients.deleteSuccess'));
        fetchPatients();
      }
    } catch (error: any) {
      message.error(t('errors.UNKNOWN_ERROR'));
    }
  };

  const handleModalClose = () => {
    setIsModalOpen(false);
    setEditingPatient(null);
  };

  const handleSuccess = () => {
    handleModalClose();
    fetchPatients();
  };

  const columns = [
    {
      title: t('patients.fullName'),
      dataIndex: 'fullName',
      key: 'fullName',
    },
    {
      title: t('patients.phoneNumber'),
      dataIndex: 'phoneNumber',
      key: 'phoneNumber',
    },
    {
      title: t('patients.email'),
      dataIndex: 'email',
      key: 'email',
    },
    {
      title: t('patients.gender'),
      dataIndex: 'gender',
      key: 'gender',
    },
    {
      title: t('common.actions'),
      key: 'actions',
      render: (_: any, record: Patient) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => handleEdit(record)}
          >
            {t('common.edit')}
          </Button>
          <Popconfirm
            title={t('patients.deleteConfirm')}
            onConfirm={() => handleDelete(record.id)}
            okText={t('common.yes')}
            cancelText={t('common.no')}
          >
            <Button type="link" danger icon={<DeleteOutlined />}>
              {t('common.delete')}
            </Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between' }}>
        <Input.Search
          placeholder={t('patients.searchPlaceholder')}
          allowClear
          enterButton={<SearchOutlined />}
          onSearch={handleSearch}
          style={{ width: 300 }}
        />
        <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
          {t('patients.addPatient')}
        </Button>
      </div>

      <Table
        columns={columns}
        dataSource={patients}
        rowKey="id"
        loading={loading}
        pagination={{
          current: page,
          pageSize,
          total,
          onChange: (newPage, newPageSize) => {
            setPage(newPage);
            setPageSize(newPageSize);
          },
          showSizeChanger: true,
          showTotal: (total) => t('common.totalItems', { total }),
        }}
      />

      <Modal
        title={editingPatient ? t('patients.editPatient') : t('patients.addPatient')}
        open={isModalOpen}
        onCancel={handleModalClose}
        footer={null}
        width={700}
      >
        <PatientForm
          patient={editingPatient}
          onSuccess={handleSuccess}
          onCancel={handleModalClose}
        />
      </Modal>
    </div>
  );
};

export default Patients;