import { useCallback, useEffect, useState } from "react";
import type { UserDetail } from "../../types/UserTypes";
import { CommunicationMethod } from "../../types/enums/CommunicationMethod";
import { SessionFrequency } from "../../types/enums/SessionFrequency";
import { LearningStyle } from "../../types/enums/LearningStyle";
import { Button, Steps } from "antd";
import UserProfile from "./components/UserProfile";
import UserPreference from "./components/UserPreference";
import { useLocation } from "react-router-dom";
import { userService } from "../../services/user/userService";

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
  const { userId, token } = state;

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

  const fetchUserDetail = useCallback(async () => {
    await userService.getUserDetail(userId).then((response) => {
      setUserDetail(response);
    });
  }, [userId]);

  useEffect(() => {
    fetchUserDetail();
  }, [fetchUserDetail]);

  const [currentStep, setCurrentStep] = useState(1);

  const nextStep = () => {
    setCurrentStep(currentStep + 1);
  };

  const prevStep = () => {
    setCurrentStep(currentStep - 1);
  };

  const onStepChange = (value: number) => {
    setCurrentStep(value);
  };

  return (
    <div className="mih-h-content">
      <Steps
        type="navigation"
        current={currentStep}
        onChange={onStepChange}
        className="site-steps p-6!"
        items={stepItems}
      />
      {currentStep === 1 ? (
        <UserProfile
          userId={userId}
          userDetail={userDetail}
          updateUserDetail={setUserDetail}
        />
      ) : (
        <UserPreference
          userId={userId}
          token={token}
          userDetail={userDetail}
          updateUserDetail={setUserDetail}
        />
      )}
      <div className="flex justify-center mb-12 gap-4 max-w-3xl m-auto">
        {currentStep > 1 && (
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
            onClick={() => console.log(userDetail)}
          >
            Done
          </Button>
        )}
      </div>
    </div>
  );
}
