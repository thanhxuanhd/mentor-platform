"use client"

import { useState, useEffect } from "react"
import { Layout, Button, Card, Row, Col, Typography, Tag, Spin } from "antd"
import { UserOutlined, BookOutlined, CheckSquareOutlined, FileOutlined } from "@ant-design/icons"

const { Content } = Layout
const { Title, Text, Paragraph } = Typography

// Định nghĩa interface cho metrics
interface DashboardMetrics {
  totalUsers: {
    count: number
    mentors: number
    learners: number
    growth: number
  }
  resources: {
    count: number
    added: number
    growth: number
  }
  sessions: {
    count: number
    weekly: number
    growth: number
  }
  pendingApprovals: {
    count: number
  }
  performance: {
    sessionRating: number
    mentorRetention: number
    resourceDownloads: number
    newUsers: number
  }
}

// Tạo service riêng cho API calls
const dashboardService = {
  getAdminMetrics: async (): Promise<DashboardMetrics> => {
    try {
      // Thay thế bằng API call thực tế
      // const response = await fetch('/api/admin/dashboard');
      // return await response.json();

      // Mock data cho demo
      return {
        totalUsers: {
          count: 330,
          mentors: 76,
          learners: 254,
          growth: 15,
        },
        resources: {
          count: 128,
          added: 14,
          growth: 8,
        },
        sessions: {
          count: 538,
          weekly: 32,
          growth: 12,
        },
        pendingApprovals: {
          count: 12,
        },
        performance: {
          sessionRating: 4.8,
          mentorRetention: 92,
          resourceDownloads: 3.2,
          newUsers: 48,
        },
      }
    } catch (error) {
      console.error("Failed to fetch admin metrics:", error)
      throw error
    }
  },
}

export default function AdminDashboard() {
  const [metrics, setMetrics] = useState<DashboardMetrics | null>(null)
  const [loading, setLoading] = useState<boolean>(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true)
        const data = await dashboardService.getAdminMetrics()
        setMetrics(data)
      } catch (err) {
        setError("Failed to load dashboard data")
        console.error("Error fetching dashboard data:", err)
      } finally {
        setLoading(false)
      }
    }

    fetchData()
  }, [])

  if (loading) {
    return (
      <div style={{ display: "flex", justifyContent: "center", alignItems: "center", height: "400px" }}>
        <Spin size="large" />
      </div>
    )
  }

  if (error || !metrics) {
    return <div style={{ color: "red", padding: "20px" }}>{error || "Something went wrong"}</div>
  }

  return (
    <Layout style={{ minHeight: "100vh" }}>
      <Layout>
        <Content style={{ background: "#1e2738", borderRadius: "8px", padding: "24px" }}>
          <div>
            <Title level={2} style={{ color: "#fff", marginBottom: "8px" }}>
              Admin Dashboard
            </Title>
            <Paragraph style={{ color: "#aaa", marginBottom: "24px" }}>
              Platform metrics and statistics overview
            </Paragraph>

            <Row gutter={[16, 16]}>
              <Col xs={24} md={12}>
                <Card style={{ background: "#2a3446", borderRadius: "8px" }} bodyStyle={{ padding: "24px" }}>
                  <div style={{ display: "flex", alignItems: "center", marginBottom: "16px" }}>
                    <UserOutlined style={{ fontSize: "20px", color: "#ff6b00", marginRight: "8px" }} />
                    <Text style={{ color: "#ff6b00", fontSize: "16px" }}>Total Users</Text>
                  </div>
                  <Title level={2} style={{ color: "#fff", margin: "0" }}>
                    {metrics.totalUsers.count}
                  </Title>
                  <div style={{ display: "flex", justifyContent: "space-between", marginTop: "8px" }}>
                    <Text style={{ color: "#aaa" }}>
                      {metrics.totalUsers.mentors} Mentors · {metrics.totalUsers.learners} Learners
                    </Text>
                    <Tag color="green">+{metrics.totalUsers.growth}% this month</Tag>
                  </div>
                </Card>
              </Col>

              <Col xs={24} md={12}>
                <Card style={{ background: "#2a3446", borderRadius: "8px" }} bodyStyle={{ padding: "24px" }}>
                  <div style={{ display: "flex", alignItems: "center", marginBottom: "16px" }}>
                    <FileOutlined style={{ fontSize: "20px", color: "#ff6b00", marginRight: "8px" }} />
                    <Text style={{ color: "#ff6b00", fontSize: "16px" }}>Resources</Text>
                  </div>
                  <Title level={2} style={{ color: "#fff", margin: "0" }}>
                    {metrics.resources.count}
                  </Title>
                  <div style={{ display: "flex", justifyContent: "space-between", marginTop: "8px" }}>
                    <Text style={{ color: "#aaa" }}>{metrics.resources.added} added this week</Text>
                    <Tag color="green">+{metrics.resources.growth}% this month</Tag>
                  </div>
                </Card>
              </Col>

              <Col xs={24} md={12}>
                <Card style={{ background: "#2a3446", borderRadius: "8px" }} bodyStyle={{ padding: "24px" }}>
                  <div style={{ display: "flex", alignItems: "center", marginBottom: "16px" }}>
                    <BookOutlined style={{ fontSize: "20px", color: "#ff6b00", marginRight: "8px" }} />
                    <Text style={{ color: "#ff6b00", fontSize: "16px" }}>Sessions</Text>
                  </div>
                  <Title level={2} style={{ color: "#fff", margin: "0" }}>
                    {metrics.sessions.count}
                  </Title>
                  <div style={{ display: "flex", justifyContent: "space-between", marginTop: "8px" }}>
                    <Text style={{ color: "#aaa" }}>{metrics.sessions.weekly} sessions this week</Text>
                    <Tag color="green">+{metrics.sessions.growth}% this month</Tag>
                  </div>
                </Card>
              </Col>

              <Col xs={24} md={12}>
                <Card style={{ background: "#2a3446", borderRadius: "8px" }} bodyStyle={{ padding: "24px" }}>
                  <div style={{ display: "flex", alignItems: "center", marginBottom: "16px" }}>
                    <CheckSquareOutlined style={{ fontSize: "20px", color: "#ff6b00", marginRight: "8px" }} />
                    <Text style={{ color: "#ff6b00", fontSize: "16px" }}>Pending Approvals</Text>
                  </div>
                  <Title level={2} style={{ color: "#fff", margin: "0" }}>
                    {metrics.pendingApprovals.count}
                  </Title>
                  <div style={{ display: "flex", justifyContent: "flex-end", marginTop: "8px" }}>
                    <Button type="link" style={{ color: "#ff6b00", padding: 0 }}>
                      Review Approvals →
                    </Button>
                  </div>
                </Card>
              </Col>
            </Row>

            <div style={{ marginTop: "32px" }}>
              <Title level={4} style={{ color: "#fff", marginBottom: "16px" }}>
                Platform Performance
              </Title>
              <Card style={{ background: "#2a3446", borderRadius: "8px" }} bodyStyle={{ padding: "24px" }}>
                <Row gutter={[32, 16]}>
                  <Col xs={24} sm={12} md={6}>
                    <div>
                      <Text style={{ color: "#aaa" }}>Avg. Session Rating</Text>
                      <div style={{ marginTop: "8px" }}>
                        <Title level={3} style={{ color: "#fff", margin: 0 }}>
                          {metrics.performance.sessionRating}/5
                        </Title>
                      </div>
                    </div>
                  </Col>
                  <Col xs={24} sm={12} md={6}>
                    <div>
                      <Text style={{ color: "#aaa" }}>Mentor Retention</Text>
                      <div style={{ marginTop: "8px" }}>
                        <Title level={3} style={{ color: "#fff", margin: 0 }}>
                          {metrics.performance.mentorRetention}%
                        </Title>
                      </div>
                    </div>
                  </Col>
                  <Col xs={24} sm={12} md={6}>
                    <div>
                      <Text style={{ color: "#aaa" }}>Resource Downloads</Text>
                      <div style={{ marginTop: "8px" }}>
                        <Title level={3} style={{ color: "#fff", margin: 0 }}>
                          {metrics.performance.resourceDownloads}K
                        </Title>
                      </div>
                    </div>
                  </Col>
                  <Col xs={24} sm={12} md={6}>
                    <div>
                      <Text style={{ color: "#aaa" }}>New Users (30d)</Text>
                      <div style={{ marginTop: "8px" }}>
                        <Title level={3} style={{ color: "#fff", margin: 0 }}>
                          +{metrics.performance.newUsers}
                        </Title>
                      </div>
                    </div>
                  </Col>
                </Row>
              </Card>
            </div>
          </div>
        </Content>
      </Layout>
    </Layout>
  )
}
