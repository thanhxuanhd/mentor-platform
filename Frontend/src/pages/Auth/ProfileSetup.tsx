import { useCallback, useEffect, useRef, useState } from "react";
import type { UserDetail } from "../../types/UserTypes";
import { CommunicationMethod } from "../../types/enums/CommunicationMethod";
import { SessionFrequency } from "../../types/enums/SessionFrequency";
import { LearningStyle } from "../../types/enums/LearningStyle";
import { App, Button, Steps, type FormInstance } from "antd";
import UserProfile from "./components/UserProfile";
import UserPreference from "./components/UserPreference";
import { useLocation, useNavigate } from "react-router-dom";
import { userService } from "../../services/user/userService";
import { axiosClient } from "../../services/apiClient";
import type { NotificationProps } from "../../types/Notification";

const stepItems: {
  status?: "finish" | "process" | "wait" | "error";
  title: string;
  disabled?: boolean;
}[] = [
  {
    status: "finish",
    title: "Step 1",
    disabled: true,
  },
  {
    title: "Step 2",
  },
  {
    title: "Step 3",
  },
];

export default function ProfileSetup() {
  const { state } = useLocation();
  const { userId } = state;
  const formRef = useRef<FormInstance<UserDetail>>(null);
  const navigate = useNavigate();
  const { notification } = App.useApp();
  const [userDetail, setUserDetail] = useState<UserDetail>({
    fullName: "",
    roleId: 0,
    bio: "",
    profilePhotoUrl: "",
    phoneNumber: "",
    skills: "",
    experiences: "",
    goal: "",
    preferredCommunicationMethod: CommunicationMethod.AudioCall,
    preferredSessionFrequency: SessionFrequency.AsNeeded,
    preferredSessionDuration: 0,
    preferredLearningStyle: LearningStyle.Auditory,
    isPrivate: false,
    isAllowedMessage: false,
    isReceiveNotification: false,
    availabilityIds: [],
    expertiseIds: [],
    teachingApproachIds: [],
    categoryIds: [],
  });
  console.log(userDetail);
  const [notify, setNotify] = useState<NotificationProps | null>(null);

  const fetchUserDetail = useCallback(async () => {
    await userService.getUserDetail(userId).then((response) => {
      setUserDetail(response);
    });
  }, [userId]);

  useEffect(() => {
    fetchUserDetail();
  }, [fetchUserDetail]);

  useEffect(() => {
    if (notify) {
      notification[notify.type]({
        message: notify.message,
        description: notify.description,
        placement: "topRight",
        showProgress: true,
        duration: 3,
        pauseOnHover: true,
      });
      setNotify(null);
    }
  }, [notify, notification]);

  const [currentStep, setCurrentStep] = useState(1);

  const nextStep = async () => {
    if (currentStep === 1) {
      try {
        await formRef.current?.validateFields();
        setCurrentStep(currentStep + 1);
      } catch {
        setNotify({
          type: "warning",
          message: "Error",
          description: "Fill all the required fields first.",
        });
      }
    } else {
      setCurrentStep(currentStep + 1);
    }
  };

  const prevStep = () => {
    if (currentStep === 1) {
      navigate("/login");
    }
    setCurrentStep(currentStep - 1);
  };

  const handleSubmit = async () => {
    try {
      await formRef.current?.validateFields();
      await axiosClient.put(`/Users/${userId}/detail`, userDetail);
      navigate("/login");
      setNotify({
        type: "success",
        message: "Success",
        description: "Your profile has been created successfully.",
      });
    } catch {
      setNotify({
        type: "error",
        message: "Error",
        description: "Failed to create new profile",
      });
    }
  };

  return (
    <div className="mih-h-content">
      <Steps
        current={currentStep}
        className="site-steps p-6!"
        items={stepItems}
      />
      {currentStep === 1 ? (
        <UserProfile
          userId={userId}
          userDetail={userDetail}
          updateUserDetail={setUserDetail}
          formRef={formRef}
        />
      ) : (
        <UserPreference
          userId={userId}
          userDetail={userDetail}
          updateUserDetail={setUserDetail}
          formRef={formRef}
        />
      )}
      <div className="flex justify-center mb-12 gap-4 max-w-3xl m-auto">
        {currentStep > 0 && (
          <Button size="large" onClick={() => prevStep()}>
            Back
          </Button>
        )}
        {currentStep < stepItems.length - 1 && (
          <Button
            type="primary"
            size="large"
            onClick={() => nextStep()}
            className="flex-1"
          >
            Next Step
          </Button>
        )}
        {currentStep === stepItems.length - 1 && (
          <Button
            type="primary"
            size="large"
            className="flex-1"
            onClick={handleSubmit}
          >
            Complete Registration
          </Button>
        )}
      </div>
    </div>
  );
}
