import { useState, useEffect } from 'react';
import { Timeline, Card, Typography, Spin, Empty, Button, Modal, Tag, Descriptions, Collapse } from 'antd';
import { EyeOutlined, CalendarOutlined, UserOutlined, MedicineBoxOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { treatmentHistoryService, type TreatmentHistory } from '../services/treatmentHistoryService';

const { Title, Text, Paragraph } = Typography;

interface PatientTreatmentHistoryProps {
  patientId: string;
  patientName?: string;
}

const PatientTreatmentHistory = ({ patientId, patientName }: PatientTreatmentHistoryProps) => {
  const [treatments, setTreatments] = useState<TreatmentHistory[]>([]);
  const [loading, setLoading] = useState(false);
  const [selectedTreatment, setSelectedTreatment] = useState<TreatmentHistory | null>(null);
  const [isDetailModalOpen, setIsDetailModalOpen] = useState(false);

  useEffect(() => {
    fetchTreatmentHistory();
  }, [patientId]);

  const fetchTreatmentHistory = async () => {
    setLoading(true);
    try {
      const response = await treatmentHistoryService.getPatientTreatmentHistory(patientId);
      if (response.success && response.data) {
        setTreatments(response.data);
      }
    } catch (error) {
      console.error('Error fetching treatment history:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleViewDetails = (treatment: TreatmentHistory) => {
    setSelectedTreatment(treatment);
    setIsDetailModalOpen(true);
  };

  const getTimelineColor = (treatment: TreatmentHistory) => {
    const daysDiff = dayjs().diff(dayjs(treatment.treatmentDate), 'days');
    if (daysDiff === 0) return 'red'; // Today
    if (daysDiff <= 7) return 'blue'; // This week
    if (daysDiff <= 30) return 'green'; // This month
    return 'gray'; // Older
  };

  const renderVitalSigns = (treatment: TreatmentHistory) => {
    const vitals = [];
    if (treatment.bloodPressure) vitals.push(`BP: ${treatment.bloodPressure}`);
    if (treatment.temperature) vitals.push(`Temp: ${treatment.temperature}°C`);
    if (treatment.heartRate) vitals.push(`HR: ${treatment.heartRate} bpm`);
    if (treatment.weight) vitals.push(`Weight: ${treatment.weight} kg`);
    
    return vitals.length > 0 ? vitals.join(' | ') : 'No vital signs recorded';
  };

  const timelineItems = treatments.map((treatment) => ({
    color: getTimelineColor(treatment),
    children: (
      <Card 
        key={treatment.id}
        size="small" 
        style={{ marginBottom: 8 }}
        actions={[
          <Button 
            key="view" 
            type="link" 
            size="small" 
            icon={<EyeOutlined />}
            onClick={() => handleViewDetails(treatment)}
          >
            View Details
          </Button>
        ]}
      >
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
          <div style={{ flex: 1 }}>
            <div style={{ display: 'flex', alignItems: 'center', marginBottom: 8 }}>
              <CalendarOutlined style={{ marginRight: 8, color: '#1890ff' }} />
              <Text strong>{dayjs(treatment.treatmentDate).format('MMM DD, YYYY HH:mm')}</Text>
              <UserOutlined style={{ marginLeft: 16, marginRight: 8, color: '#52c41a' }} />
              <Text>{treatment.staffName}</Text>
            </div>
            
            {treatment.chiefComplaint && (
              <div style={{ marginBottom: 4 }}>
                <Text strong>Chief Complaint: </Text>
                <Text>{treatment.chiefComplaint}</Text>
              </div>
            )}
            
            {treatment.diagnosis && (
              <div style={{ marginBottom: 4 }}>
                <Text strong>Diagnosis: </Text>
                <Text>{treatment.diagnosis}</Text>
              </div>
            )}
            
            <div style={{ marginBottom: 4 }}>
              <Text strong>Treatment: </Text>
              <Text>{treatment.treatment}</Text>
            </div>
            
            <div style={{ fontSize: '12px', color: '#666' }}>
              <Text type="secondary">{renderVitalSigns(treatment)}</Text>
            </div>
          </div>
          
          {treatment.nextAppointmentDate && (
            <Tag color="orange" style={{ marginLeft: 8 }}>
              Next: {dayjs(treatment.nextAppointmentDate).format('MMM DD')}
            </Tag>
          )}
        </div>
      </Card>
    ),
  }));

  if (loading) {
    return (
      <div style={{ textAlign: 'center', padding: 50 }}>
        <Spin size="large" />
      </div>
    );
  }

  if (treatments.length === 0) {
    return (
      <Empty
        image={<MedicineBoxOutlined style={{ fontSize: 64, color: '#d9d9d9' }} />}
        description="No treatment history found"
      />
    );
  }

  return (
    <div>
      <div style={{ marginBottom: 16 }}>
        <Title level={4}>
          Treatment History {patientName && `- ${patientName}`}
        </Title>
        <Text type="secondary">
          {treatments.length} treatment record{treatments.length !== 1 ? 's' : ''} found
        </Text>
      </div>

      <Timeline items={timelineItems} />

      {/* Treatment Detail Modal */}
      <Modal
        title="Treatment Details"
        open={isDetailModalOpen}
        onCancel={() => {
          setIsDetailModalOpen(false);
          setSelectedTreatment(null);
        }}
        footer={null}
        width={800}
      >
        {selectedTreatment && (
          <div>
            <Descriptions 
              title="Basic Information"
              bordered
              column={2}
              size="small"
              style={{ marginBottom: 16 }}
            >
              <Descriptions.Item label="Date & Time">
                {dayjs(selectedTreatment.treatmentDate).format('MMMM DD, YYYY HH:mm')}
              </Descriptions.Item>
              <Descriptions.Item label="Attending Staff">
                {selectedTreatment.staffName}
              </Descriptions.Item>
            </Descriptions>

            <Collapse
              size="small"
              items={[
                {
                  key: '1',
                  label: 'Chief Complaint & Symptoms',
                  children: (
                    <div>
                      {selectedTreatment.chiefComplaint && (
                        <div style={{ marginBottom: 8 }}>
                          <Text strong>Chief Complaint:</Text>
                          <Paragraph style={{ marginTop: 4 }}>{selectedTreatment.chiefComplaint}</Paragraph>
                        </div>
                      )}
                      {selectedTreatment.symptoms && (
                        <div>
                          <Text strong>Symptoms:</Text>
                          <Paragraph style={{ marginTop: 4 }}>{selectedTreatment.symptoms}</Paragraph>
                        </div>
                      )}
                    </div>
                  ),
                },
                {
                  key: '2',
                  label: 'Vital Signs',
                  children: (
                    <Descriptions column={3} size="small">
                      <Descriptions.Item label="Blood Pressure">
                        {selectedTreatment.bloodPressure || 'Not recorded'}
                      </Descriptions.Item>
                      <Descriptions.Item label="Temperature">
                        {selectedTreatment.temperature ? `${selectedTreatment.temperature}°C` : 'Not recorded'}
                      </Descriptions.Item>
                      <Descriptions.Item label="Heart Rate">
                        {selectedTreatment.heartRate ? `${selectedTreatment.heartRate} bpm` : 'Not recorded'}
                      </Descriptions.Item>
                      <Descriptions.Item label="Respiratory Rate">
                        {selectedTreatment.respiratoryRate ? `${selectedTreatment.respiratoryRate}` : 'Not recorded'}
                      </Descriptions.Item>
                      <Descriptions.Item label="Weight">
                        {selectedTreatment.weight ? `${selectedTreatment.weight} kg` : 'Not recorded'}
                      </Descriptions.Item>
                      <Descriptions.Item label="Height">
                        {selectedTreatment.height ? `${selectedTreatment.height} cm` : 'Not recorded'}
                      </Descriptions.Item>
                    </Descriptions>
                  ),
                },
                {
                  key: '3',
                  label: 'Examination & Diagnosis',
                  children: (
                    <div>
                      {selectedTreatment.physicalExamination && (
                        <div style={{ marginBottom: 8 }}>
                          <Text strong>Physical Examination:</Text>
                          <Paragraph style={{ marginTop: 4 }}>{selectedTreatment.physicalExamination}</Paragraph>
                        </div>
                      )}
                      {selectedTreatment.diagnosis && (
                        <div style={{ marginBottom: 8 }}>
                          <Text strong>Primary Diagnosis:</Text>
                          <Paragraph style={{ marginTop: 4 }}>{selectedTreatment.diagnosis}</Paragraph>
                        </div>
                      )}
                      {selectedTreatment.differentialDiagnosis && (
                        <div>
                          <Text strong>Differential Diagnosis:</Text>
                          <Paragraph style={{ marginTop: 4 }}>{selectedTreatment.differentialDiagnosis}</Paragraph>
                        </div>
                      )}
                    </div>
                  ),
                },
                {
                  key: '4',
                  label: 'Treatment & Prescription',
                  children: (
                    <div>
                      <div style={{ marginBottom: 8 }}>
                        <Text strong>Treatment Provided:</Text>
                        <Paragraph style={{ marginTop: 4 }}>{selectedTreatment.treatment}</Paragraph>
                      </div>
                      {selectedTreatment.prescriptionNotes && (
                        <div style={{ marginBottom: 8 }}>
                          <Text strong>Prescription:</Text>
                          <Paragraph style={{ marginTop: 4 }}>{selectedTreatment.prescriptionNotes}</Paragraph>
                        </div>
                      )}
                      {selectedTreatment.treatmentPlan && (
                        <div>
                          <Text strong>Treatment Plan:</Text>
                          <Paragraph style={{ marginTop: 4 }}>{selectedTreatment.treatmentPlan}</Paragraph>
                        </div>
                      )}
                    </div>
                  ),
                },
                {
                  key: '5',
                  label: 'Follow-up & Notes',
                  children: (
                    <div>
                      {selectedTreatment.followUpInstructions && (
                        <div style={{ marginBottom: 8 }}>
                          <Text strong>Follow-up Instructions:</Text>
                          <Paragraph style={{ marginTop: 4 }}>{selectedTreatment.followUpInstructions}</Paragraph>
                        </div>
                      )}
                      {selectedTreatment.nextAppointmentDate && (
                        <div style={{ marginBottom: 8 }}>
                          <Text strong>Next Appointment:</Text>
                          <Paragraph style={{ marginTop: 4 }}>
                            {dayjs(selectedTreatment.nextAppointmentDate).format('MMMM DD, YYYY HH:mm')}
                          </Paragraph>
                        </div>
                      )}
                      {selectedTreatment.notes && (
                        <div>
                          <Text strong>Additional Notes:</Text>
                          <Paragraph style={{ marginTop: 4 }}>{selectedTreatment.notes}</Paragraph>
                        </div>
                      )}
                    </div>
                  ),
                },
              ]}
              defaultActiveKey={['1', '2', '3', '4', '5']}
            />
          </div>
        )}
      </Modal>
    </div>
  );
};

export default PatientTreatmentHistory;