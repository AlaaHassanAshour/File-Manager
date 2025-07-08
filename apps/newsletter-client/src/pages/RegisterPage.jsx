import { useContext, useState } from "react";
import { Button, Col, Form, Input, Row, Typography, theme } from "antd";

import { useThemeMode } from "../contexts/ThemeMode";
import Auth from "../contexts/Auth";
import { notification } from "../utils/InitAntStaticApi";
import { AUTH_CONFIG, APP_CONFIG } from "../config/env";
import { register } from "../services/api";
import { useNavigate  } from "react-router-dom";

const { Title } = Typography;

const RegisterPage = () => {
  const { setAuth } = useContext(Auth);
  const { darkMode } = useThemeMode();
  const [loading, setLoading] = useState(false);
  const [form] = Form.useForm();
  const { token } = theme.useToken();
const navigate = useNavigate()
  const handleRegister = async (formData) => {
    try {
      setLoading(true);
      const response = await register(formData); // افترض وجود register API
     navigate('/login')
      notification.success({
        message: "Registration Successful",
        description: "Your account has been created successfully.",
      });
    } catch (error) {
      notification.error({
        message: "Registration Failed",
        description: error?.message || "Something went wrong.",
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <Row
      style={{ height: "100%", padding: "20px" }}
      align="middle"
      justify="center"
    >
      <Row
        style={{
          width: "600px",
          padding: "40px",
          borderRadius: "8px",
          boxShadow: darkMode
            ? `0 2px 8px ${token.colorBgElevated}57`
            : "0 2px 8px #adadad57",
          background: token.colorBgElevated,
        }}
      >
        <Col span={24}>
          <Title
            level={2}
            style={{ textAlign: "center", marginBottom: "30px" }}
          >
            Register
          </Title>
        </Col>
        <Col span={24}>
          <Form
            form={form}
            name="registerForm"
            labelCol={{ span: 7 }}
            wrapperCol={{ span: 17 }}
            onFinish={handleRegister}
            autoComplete="off"
          >
            <Form.Item
              name="email"
              label="Email"
              rules={[
                { required: true, message: "Email is required" },
                { type: "email" },
              ]}
            >
              <Input placeholder="Enter your email" disabled={loading} />
            </Form.Item>

            <Form.Item
              name="username"
              label="Username"
              rules={[
                { required: true, message: "Username is required" },
                { min: 3, message: "Minimum 3 characters" },
              ]}
            >
              <Input placeholder="Enter your username" disabled={loading} />
            </Form.Item>

            <Form.Item
              name="password"
              label="Password"
              rules={[
                { required: true, message: "Password is required" },
                { min: 6, message: "Minimum 6 characters" },
              ]}
            >
              <Input.Password placeholder="Enter password" disabled={loading} />
            </Form.Item>

            <Form.Item
              wrapperCol={{
                offset: 7,
                span: 24,
              }}
            >
              <Button
                style={{ width: "100px", marginRight: "10px" }}
                loading={loading}
                type="primary"
                htmlType="submit"
                disabled={loading}
              >
                Register
              </Button>
            </Form.Item>
          </Form>
        </Col>
      </Row>
    </Row>
  );
};

export default RegisterPage;
