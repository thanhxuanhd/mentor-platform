import { Spin } from 'antd';

const Loading = ({ message = 'Loading...' }) => {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen">
      <Spin
        size='large'
        tip={message}
        className="mb-4"
      />
    </div>
  );
};

export default Loading;