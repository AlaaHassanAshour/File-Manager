import { Col, Layout, Row } from "antd";
import { Content, Header } from "antd/es/layout/layout";

import DarkModeSwitch from "../components/DarkModeSwitch";
import LocalizationButton from "../components/LocalizationButton";
import { Link } from "react-router-dom";
import { UserAddOutlined } from "@ant-design/icons"; // أيقونة تسجيل جديد

export default function UnprotectedRoute({ children }) {
  return (
    <Layout>
      <Header>
        <Row
          style={{ height: "100%" }}
          justify="end"
          align="middle"
          gutter={[10]}
        >
          <Col>
            <DarkModeSwitch />
          </Col>
          <Col style={{ display: "flex" }}>
            <LocalizationButton />
          </Col>
              <Col>
            <Link to="/register" style={{ color: "white", fontSize: "18px" }}>
              <UserAddOutlined />
            </Link>
          </Col>
        </Row>
      </Header>
      <Content style={{ height: "calc(100vh - 64px)" }}>{children}</Content>
    </Layout>
  );
}
